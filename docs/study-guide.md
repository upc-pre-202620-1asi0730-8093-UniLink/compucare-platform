# CompuCare Platform - Study Guide

## Table of Contents

1. [Overview](#1-overview)
2. [Architecture Patterns](#2-architecture-patterns)
3. [Domain-Driven Design (DDD)](#3-domain-driven-design-ddd)
4. [CQRS Pattern](#4-cqrs-pattern)
5. [.NET Framework Features](#5-net-framework-features)
6. [C# Language Features](#6-c-language-features)

---

## 1. Overview

CompuCare Platform is the REST API behind UniLink's CompuCare service: companies register their devices, subscribe to a monthly bank of support hours, and their employees report issues that UniLink technicians diagnose, quote, and resolve.

### Technology Stack

- .NET 10 / C# 14, ASP.NET Core Web API with controllers
- Entity Framework Core 10 with MySQL (`MySql.EntityFrameworkCore`)
- Cortex.Mediator, BCrypt.Net-Next, System.IdentityModel.Tokens.Jwt
- Microsoft.AspNetCore.OpenApi + Scalar
- `.resx` localization (`en`, `es`)
- xUnit

### Key Architectural Approaches

- Modular monolith: one deployable project split into bounded contexts.
- Four layers per context: `Domain`, `Application`, `Infrastructure`, `Interfaces`.
- CQRS with command and query services.
- Functional `Result<T>` instead of exceptions for expected failures.
- Anti-corruption layer facades between contexts.

### Bounded Contexts

| Context | Responsibility | Aggregates |
| --- | --- | --- |
| `Iam` | Accounts, roles, sign-in, JWT, password recovery | `User`, `PasswordResetToken` |
| `Companies` | Companies, locations, equipment, assignments | `Company`, `Location`, `Equipment` |
| `Subscriptions` | Plans, support-hour balance, preventive visits | `Plan`, `Subscription`, `MaintenanceVisit` |
| `ServiceRequests` | Ticket lifecycle | `ServiceRequest` |
| `Quotations` | Quotations and simulated payment | `Quotation` |
| `Notifications` | In-app messages | `Notification` |
| `Shared` | Base abstractions and technical plumbing | `AggregateRoot` (base class) |

---

## 2. Architecture Patterns

### 2.1 Clean Architecture

Dependencies point inward. `Domain` knows nothing about EF Core or HTTP; `Application` orchestrates use cases through repository interfaces; `Infrastructure` implements those interfaces; `Interfaces` translates HTTP into commands and queries.

```text
ServiceRequests/
  Domain/          Model/{Aggregates, Entities, ValueObjects, Commands, Queries, Errors}, Repositories
  Application/     CommandServices, QueryServices, Internal/{CommandServices, QueryServices, OutboundServices/Acl}, Acl
  Infrastructure/  Persistence/EntityFrameworkCore/{Configuration/Extensions, Repositories}
  Interfaces/      Rest/{Resources, Transform}, Acl
```

### 2.2 Repository Pattern

Each aggregate has a repository interface in `Domain/Repositories` that extends `IBaseRepository<TEntity>`, and an EF Core implementation in `Infrastructure` that extends `BaseRepository<TEntity>`.

```csharp
public interface IServiceRequestRepository : IBaseRepository<ServiceRequest>
{
    Task<IEnumerable<ServiceRequest>> FindByCompanyIdAsync(int companyId, CancellationToken cancellationToken);
}
```

### 2.3 Unit of Work Pattern

`IUnitOfWork.CompleteAsync` saves every change tracked by the request's `AppDbContext`. Because all contexts share it, a command that changes several contexts — resolving a ticket consumes subscription hours and creates notifications — is committed once, atomically (ADR-006).

### 2.4 Dependency Injection

`Program.cs` registers every repository, service, facade, and outbound ACL service per bounded context with `AddScoped`. Outbound ACL services with the same class name in different contexts are registered through namespace aliases (`ServiceRequestsAcl.ExternalUserService`, `QuotationsAcl.ExternalUserService`).

### 2.5 Middleware Pattern

`RequestAuthorizationMiddleware` validates the bearer token, loads the active user, and stores it on `HttpContext.Items`. The `[Authorize]` filter then returns 401 or, when roles are given, 403. `GlobalExceptionHandler` turns unexpected exceptions into problem details.

### 2.6 Assembler Pattern

Static assemblers in `Interfaces/Rest/Transform` keep translation out of controllers:
- `XxxCommandFromResourceAssembler` builds a command from a request resource and the authenticated user.
- `XxxResourceFromEntityAssembler` builds a response resource from an aggregate.
- `XxxActionResultAssembler` maps a `Result<T>` failure to a problem-details response with the right status code.

### 2.7 Mediator Pattern with Pipeline Behaviors

`Cortex.Mediator` publishes domain events through `DispatchDomainEventsInterceptor` after `SaveChanges`. The logging, exception-handling, and transaction pipeline behaviors are registered as in the reference project.

### 2.8 Service Layer Pattern

Command services implement one method per command (`Handle(ResolveServiceRequestCommand ...)`), validate cross-aggregate rules, call aggregate methods, and commit. Query services only read.

---

## 3. Domain-Driven Design (DDD)

### 3.1 Bounded Contexts

Each context owns its data and rules. Another context's aggregates are never referenced: a ticket stores `EquipmentId` as an `int` and asks the Companies facade whether that device is assigned to the requester.

### 3.2 Aggregate Roots

Aggregates protect their invariants in their methods. `ServiceRequest` is the clearest example: it can only be assigned while pending, only its technician can start or resolve it, and it cannot be resolved while a quotation is unresolved.

```csharp
public void StartDiagnosis(int technicianId)
{
    if (Status != EServiceRequestStatus.Assigned || technicianId != TechnicianId)
        throw new InvalidOperationException("Only the assigned technician can start the diagnosis.");
    Status = EServiceRequestStatus.InDiagnosis;
    Log(Status);
}
```

Internal collections (`TimelineEntry`, `CoverageConsumption`, `QuotationLine`, `EquipmentAssignment`) are exposed as read-only and change only through the root.

### 3.3 Value Objects

Statuses are enumerations (`EServiceRequestStatus`, `EQuotationStatus`, `EEquipmentStatus`). Their display labels (`Pendiente de Asignación`, `APPROVED`, `En Reparación`) are produced by assemblers, so persisted values do not depend on interface text.

### 3.4 Strongly-Typed IDs

```csharp
public readonly record struct ServiceRequestId(int Value) : IEntityId<int>
{
    public static implicit operator int(ServiceRequestId id) => id.Value;
    public static implicit operator ServiceRequestId(int value) => new(value);
}
```

Mapped with `StronglyTypedIdConverter` and generated by the database (`ValueGeneratedOnAdd`).

### 3.5 Domain Events

`AggregateRoot` stores domain-event factories that `DispatchDomainEventsInterceptor` publishes after the changes are saved, so events carry the real database-generated ids.

### 3.6 Anti-Corruption Layer (ACL)

```text
ServiceRequestCommandService
  -> ExternalSubscriptionService            (ServiceRequests/Application/Internal/OutboundServices/Acl)
    -> ISubscriptionsContextFacade          (Subscriptions/Interfaces/Acl)
      -> SubscriptionsContextFacade         (Subscriptions/Application/Acl)
        -> ISubscriptionRepository
```

The facade returns `SupportHoursConsumption`, a record of the Subscriptions published language, never the `Subscription` aggregate.

### 3.7 Error Catalogs

Each context has an error enum (`ServiceRequestsError`) and a static catalog of `Error` records (`ServiceRequestsErrors`). The enum name is also the key of the localized message in `Resources/Errors/ErrorMessages.resx` and `ErrorMessages.es.resx`.

---

## 4. CQRS Pattern

### 4.1 Command Query Responsibility Segregation

Commands change state and return `Result<T>`; queries return aggregates or read models (`ServiceRequestDetails`, `EquipmentWithStatus`, `EquipmentHistory`) and never modify anything.

### 4.2 Commands & Queries as Records

```csharp
public record ResolveServiceRequestCommand(int ServiceRequestId, int TechnicianId, string Diagnosis,
    string WorkDone, decimal Hours);

public record GetServiceRequestsByTechnicianIdQuery(int TechnicianId);
```

---

## 5. .NET Framework Features

### 5.1 ASP.NET Core Web API

Controllers use `[ApiController]`, `[Route("api/v1/[controller]")]` with a kebab-case route convention, and declare `EndpointSummary`, `EndpointDescription`, and `ProducesResponseType` on each action.

### 5.2 Generic Host & Minimal Hosting

`Program.cs` builds the host with `WebApplication.CreateBuilder`, applies `Database.Migrate()` at startup, and configures the pipeline: forwarded headers, exception handler, localization, OpenAPI/Scalar, CORS, request authorization, and controllers.

### 5.3 Configuration System

`appsettings.json`, `appsettings.Development.json`, and `appsettings.Production.json`. Production values use `%VARIABLE%` placeholders resolved with `Environment.ExpandEnvironmentVariables` (`JWT_SECRET`, `DATABASE_URL`, `DATABASE_PORT`, `DATABASE_USER`, `DATABASE_PASSWORD`, `DATABASE_SCHEMA`). `TokenSettings` and `Smtp` are bound with the options pattern.

### 5.4 Localization

`RequestLocalizationOptions` supports `en` (default) and `es`; `IStringLocalizer<ErrorMessages>` resolves the message of each error enum from the culture of the `Accept-Language` header.

### 5.5 OpenAPI & Scalar

`AddOpenApi` with `BearerSecuritySchemeTransformer`, served by `MapOpenApi` and `MapScalarApiReference` at `/scalar/v1`.

### 5.6 CORS Policy

`AllowAllPolicy` allows any origin, method, and header, so the web application can call the API from its own domain.

---

## 6. C# Language Features

### 6.1 Records

Commands, queries, resources, published-language types, and `Error` are records: immutable, with value equality.

### 6.2 Primary Constructors

Services, repositories, and controllers receive their dependencies through primary constructors:

```csharp
public class QuotationQueryService(IQuotationRepository quotationRepository) : IQuotationQueryService
```

### 6.3 Pattern Matching

```csharp
IEnumerable<ServiceRequestDetails> serviceRequests = user.Role switch
{
    UserRoles.SystemAdministrator => await serviceRequestQueryService.Handle(new GetAllServiceRequestsQuery(), ct),
    UserRoles.Technician => await serviceRequestQueryService.Handle(new GetServiceRequestsByTechnicianIdQuery(user.UserId), ct),
    _ => []
};
```

### 6.4 Nullable Reference Types

Enabled project-wide; optional values are explicit (`int? TechnicianId`, `string? Receipt`).

### 6.5 File-Scoped Namespaces

Every file declares `namespace CompuCare.Platform.<Context>.<Layer>...;`.

### 6.6 Collection Expressions

`private readonly List<TimelineEntry> _timeline = [];` and `return [];` for empty results.
