# Architecture Decision Records (ADRs)

This document records the key architectural decisions made for the **CompuCare Platform**, detailing the context, options considered, decisions, and consequences.

---

## ADR Index

1. [ADR-001: Adoption of Domain-Driven Design (DDD) and Tactical Patterns](#adr-001-adoption-of-domain-driven-design-ddd-and-tactical-patterns)
2. [ADR-002: Command Query Responsibility Segregation (CQRS)](#adr-002-command-query-responsibility-segregation-cqrs)
3. [ADR-003: Functional Result Pattern and Discriminated Error Model](#adr-003-functional-result-pattern-and-discriminated-error-model)
4. [ADR-004: Strongly-Typed Identifiers with Zero-Allocation Record Structs](#adr-004-strongly-typed-identifiers-with-zero-allocation-record-structs)
5. [ADR-005: Anti-Corruption Layer Between Bounded Contexts](#adr-005-anti-corruption-layer-between-bounded-contexts)
6. [ADR-006: One Unit of Work for Cross-Context Commands](#adr-006-one-unit-of-work-for-cross-context-commands)
7. [ADR-007: Scalar API Reference and Built-in ASP.NET Core OpenAPI](#adr-007-scalar-api-reference-and-built-in-aspnet-core-openapi)
8. [ADR-008: IAM Middleware with Role-Aware Authorization](#adr-008-iam-middleware-with-role-aware-authorization)
9. [ADR-009: RFC 9457 Problem Details with a Readable Message](#adr-009-rfc-9457-problem-details-with-a-readable-message)
10. [ADR-010: Keep the Web Application's HTTP Contract](#adr-010-keep-the-web-applications-http-contract)
11. [ADR-011: MySQL Persistence with Owned Collections and Short Identifiers](#adr-011-mysql-persistence-with-owned-collections-and-short-identifiers)
12. [ADR-012: Development-Only Demo Data and Simulated Email](#adr-012-development-only-demo-data-and-simulated-email)

---

## ADR-001: Adoption of Domain-Driven Design (DDD) and Tactical Patterns

- **Status:** Accepted

### Context
CompuCare coordinates several business areas with their own rules: companies and their devices, support-hour subscriptions, the ticket lifecycle, quotations, and account access. Mixing them in flat CRUD services spreads the rules across controllers and couples every module to every other.

### Decision
Structure the platform into explicit **Bounded Contexts** (`Iam`, `Companies`, `Subscriptions`, `ServiceRequests`, `Quotations`, `Notifications`, `Shared`), each with `Domain`, `Application`, `Infrastructure`, and `Interfaces` layers:
- **Aggregates & Aggregate Roots:** enforce invariants (`ServiceRequest`, `Subscription`, `Quotation`, `Equipment`, `User`, ...).
- **Entities owned by an aggregate:** `TimelineEntry`, `CoverageConsumption`, `QuotationLine`, `EquipmentAssignment` change only through their root.
- **Value Objects:** strongly-typed identifiers and enumerations such as `EServiceRequestStatus` and `EQuotationStatus`.

Strategic classification: `ServiceRequests` and `Subscriptions` are core; `Companies` and `Quotations` are supporting; `Iam` and `Notifications` are generic.

### Consequences
- **Positive:** clear domain boundaries, testable business invariants, and modular maintainability.
- **Negative:** more scaffolding and mapping between REST resources, commands, and aggregates.

---

## ADR-002: Command Query Responsibility Segregation (CQRS)

- **Status:** Accepted

### Context
Mixing reads and writes inside one large service class makes it hard to see which operations change state and which only read it.

### Decision
Split every aggregate's application services into a **Command Service** (e.g. `ServiceRequestCommandService`) and a **Query Service** (e.g. `ServiceRequestQueryService`). Commands and queries are immutable records in `Domain/Model/Commands` and `Domain/Model/Queries`. Commands read tracked aggregates; queries read with `AsNoTracking`.

### Consequences
- **Positive:** read paths never risk persisting accidental changes, and each command is a single, named use case.
- **Negative:** as in the reference project, controllers call the command services directly instead of `IMediator`, so the `Cortex.Mediator` pipeline behaviors are registered but do not run for commands; each command service calls `IUnitOfWork.CompleteAsync` itself.

---

## ADR-003: Functional Result Pattern and Discriminated Error Model

- **Status:** Accepted

### Context
Throwing exceptions for expected business outcomes (a duplicate serial number, a ticket that cannot be closed yet) turns normal flows into 500 responses unless every exception is mapped by hand.

### Decision
Command services return `Result<T>` (`Shared.Application.Model`) with an `Error` (`Shared.Domain.Model.Errors`) and a per-context error enum (`IamError`, `CompaniesError`, `SubscriptionsError`, `ServiceRequestsError`, `QuotationsError`, `NotificationsError`). Aggregates still guard their invariants by throwing `ArgumentException` (invalid data) or `InvalidOperationException` (invalid state transition); the command service catches them and returns the matching failure. Each context's `ActionResultAssembler` maps its error enum to an HTTP status code.

### Consequences
- **Positive:** every expected failure becomes a precise 400, 403, 404, or 409 response; aggregates stay free of HTTP concerns.
- **Negative:** callers must unpack the result explicitly.

---

## ADR-004: Strongly-Typed Identifiers with Zero-Allocation Record Structs

- **Status:** Accepted

### Context
Passing raw `int` ids makes it easy to swap a ticket id for a quotation id without any compiler error.

### Decision
Every aggregate root has its own `readonly record struct` id implementing `IEntityId<int>` (`UserId`, `CompanyId`, `EquipmentId`, `PlanId`, `ServiceRequestId`, `QuotationId`, ...), persisted with `StronglyTypedIdConverter`. References to aggregates of **another** bounded context are stored as plain `int`, because one context must not depend on another context's identifier types.

### Consequences
- **Positive:** type safety inside each context with no allocation cost; ids are database-generated integers.
- **Negative:** ids cross context boundaries as primitives, so their meaning is documented by the facade method names.

---

## ADR-005: Anti-Corruption Layer Between Bounded Contexts

- **Status:** Accepted

### Context
Several use cases span contexts: closing a ticket consumes subscription hours and checks quotations; issuing a quotation needs the ticket; the equipment history gathers tickets, quotations, and maintenance visits.

### Decision
Each context publishes a facade in `Interfaces/Acl` (`IIamContextFacade`, `ICompaniesContextFacade`, `ISubscriptionsContextFacade`, `IServiceRequestsContextFacade`, `IQuotationsContextFacade`, `INotificationsContextFacade`) implemented in `Application/Acl`. Consumers call it only through an outbound service in their own `Application/Internal/OutboundServices/Acl` (e.g. `ExternalSubscriptionService`). Data crosses the boundary as primitives or published-language records (`ServiceRequestSummary`, `QuotationSummary`, `MaintenanceVisitSummary`, `SupportHoursConsumption`, `QuotableServiceRequest`, `AuthenticatedUser`), never as another context's aggregate.

### Consequences
- **Positive:** a context can change its internal model without breaking its consumers; dependencies between contexts are explicit and few.
- **Negative:** one extra translation step per cross-context call.

---

## ADR-006: One Unit of Work for Cross-Context Commands

- **Status:** Accepted

### Context
Resolving a ticket changes three contexts at once: the ticket, the subscription balance, and the notifications for the company managers. Saving each part separately could leave hours consumed for a ticket that failed to close.

### Decision
All repositories share the request-scoped `AppDbContext`. Facade operations that change state (`RecordSupportHoursConsumption`, `SetServiceRequestPaused`, `Notify`) only track their changes; the command service that started the use case commits everything with a single `IUnitOfWork.CompleteAsync`. `Subscription.Version` is an optimistic concurrency token.

### Consequences
- **Positive:** a cross-context use case is atomic, and concurrent consumption of the same balance is detected.
- **Negative:** this relies on the modular monolith sharing one database; splitting a context into its own service would require integration events instead.

---

## ADR-007: Scalar API Reference and Built-in ASP.NET Core OpenAPI

- **Status:** Accepted

### Context
The web application team needs an always-current, interactive description of the API.

### Decision
Use `Microsoft.AspNetCore.OpenApi` with `BearerSecuritySchemeTransformer` and serve it through `Scalar.AspNetCore`, mapped regardless of environment. Every endpoint declares `EndpointSummary`, `EndpointDescription`, `ProducesResponseType`, and an `Accept-Language` header.

### Consequences
- **Positive:** endpoints can be tried with a bearer token from the browser, locally and after deployment.
- **Negative:** the API reference is public on any deployment of this configuration.

---

## ADR-008: IAM Middleware with Role-Aware Authorization

- **Status:** Accepted

### Context
CompuCare has five roles (`sysadmin`, `technician`, `admin`, `company_manager`, `employee`), and every company-scoped operation must stay inside the signed-in user's company.

### Decision
`RequestAuthorizationMiddleware` validates the JWT, loads the active user, and places it on the request, together with an `AuthenticatedUser` record for the other contexts. The IAM `[Authorize]` attribute accepts optional roles (`[Authorize(UserRoles.Administrator, UserRoles.CompanyManager)]`) and returns 401 without a session or 403 for another role. Controllers take the user id and company id from `AuthenticatedUser`, never from the request body.

### Consequences
- **Positive:** a browser cannot claim another role or company; disabled accounts lose access on their next request.
- **Negative:** every authenticated request reads the user from the database.

---

## ADR-009: RFC 9457 Problem Details with a Readable Message

- **Status:** Accepted

### Context
The web application shows `error.response.data.message` to the user, while ASP.NET Core problem details carry the text in `title` and `detail`.

### Decision
Errors are written as RFC 9457 problem details by `ProblemDetailsFactory` and `GlobalExceptionHandler`, with the localized text (`Resources/Errors/ErrorMessages.resx` and `.es.resx`) in `title` and `detail`, plus a `message` extension holding the same text. The web application sends its interface language in `Accept-Language`.

### Consequences
- **Positive:** standard error bodies that the web application can display without changes, in English or Spanish.
- **Negative:** the message appears twice in each error body.

---

## ADR-010: Keep the Web Application's HTTP Contract

- **Status:** Accepted

### Context
The Vue web application already calls routes such as `/api/v1/auth/login`, `/api/v1/tickets`, and `/api/v1/quotes`, and reads status labels such as `Pendiente de Asignación` and `APPROVED`.

### Decision
Keep those routes and JSON names. Where a route name differs from the bounded context (`tickets` for `ServiceRequests`, `quotes` for `Quotations`, `auth` for authentication), the controller declares the route explicitly; otherwise `[controller]` with the kebab-case convention is used. Status labels are produced by assemblers from the domain enumerations, so the persisted values never depend on interface wording.

### Consequences
- **Positive:** the web application works against this API without changes.
- **Negative:** some controller names differ from their routes.

---

## ADR-011: MySQL Persistence with Owned Collections and Short Identifiers

- **Status:** Accepted

### Context
The schema uses snake_case plural names generated by `UseSnakeCaseNamingConvention`. MySQL limits identifiers to 64 characters and has no array columns.

### Decision
Map the internal collections of an aggregate as owned types. Name the ticket timeline entity `TimelineEntry` and give the coverage-consumption unique index an explicit short name, so every generated constraint and index name stays under 64 characters. Store the device ids of a maintenance visit as a comma-separated column with a value comparer. Seed the published plan catalog in the initial migration with `HasData`.

### Consequences
- **Positive:** `Database.Migrate()` creates the whole schema, including the plans, on any MySQL 8 server.
- **Negative:** the device ids of a maintenance visit cannot be queried with SQL joins.

---

## ADR-012: Development-Only Demo Data and Simulated Email

- **Status:** Accepted

### Context
Demonstrating the four roles requires accounts, but publishing default credentials is unsafe. Employee invitations and password recovery need email.

### Decision
`DemoDataSeeder` runs only in `Development` when `DemoSeed:Enabled` is true, with a password of at least twelve characters taken from local configuration. `EmailService` sends through SMTP in other environments and fails explicitly when SMTP is not configured; in `Development` it sends nothing and the API returns the temporary password or reset token instead.

### Consequences
- **Positive:** no credentials in the repository, and the full flow can be exercised locally without an SMTP server.
- **Negative:** a production deployment must configure SMTP before inviting employees.
