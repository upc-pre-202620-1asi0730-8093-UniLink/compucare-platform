# CompuCare Platform (`compucare-platform`)

[![.NET](https://img.shields.io/badge/.NET-10-purple.svg)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-14-blue.svg)](https://learn.microsoft.com/dotnet/csharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.md)

`compucare-platform` is an ASP.NET Core REST API for CompuCare, UniLink's technical support service for small and medium companies: company accounts, equipment inventory, support-hour subscriptions, service requests (tickets), quotations, and in-app notifications. The project applies Domain-Driven Design across seven bounded contexts, with a command/query service pair per context and Entity Framework Core persistence.

---

## Domain-Driven Design approach

- **Bounded contexts**
  - `IAM`: company registration, sign-in, employees, technicians, profile and password management, issuing JWTs for authenticated access (see [ADR-008](docs/adrs.md#adr-008-iam-middleware-with-role-aware-authorization)).
  - `Companies`: companies, their locations, and their equipment, including the assignment history of every device.
  - `Subscriptions`: the plan catalog, each company's subscription, support-hour consumption, and preventive maintenance visits.
  - `ServiceRequests`: the ticket lifecycle — report, assign, diagnose, resolve, cancel — and its timeline.
  - `Quotations`: quotations issued by technicians, their approval, and the simulated payment receipt.
  - `Notifications`: in-app messages for each user.
  - `Shared`: cross-cutting abstractions every other context depends on — the aggregate-root base class, `Result<T>`/`Error` (see [ADR-003](docs/adrs.md#adr-003-functional-result-pattern-and-discriminated-error-model)), the EF Core base repository, the audit and domain-event interceptors, and the REST-layer problem-details plumbing (see [ADR-009](docs/adrs.md#adr-009-rfc-9457-problem-details-with-a-readable-message)).
- **Domain layer**: aggregate roots, entities, value objects, and strongly-typed identifiers (see [ADR-004](docs/adrs.md#adr-004-strongly-typed-identifiers-with-zero-allocation-record-structs)).
- **Application layer**: one command service and one query service per aggregate (see [ADR-002](docs/adrs.md#adr-002-command-query-responsibility-segregation-cqrs)), and an anti-corruption layer between contexts (see [ADR-005](docs/adrs.md#adr-005-anti-corruption-layer-between-bounded-contexts)).
- **Interfaces layer**: REST controllers and resources, documented through ASP.NET Core's built-in OpenAPI generation and Scalar (see [ADR-007](docs/adrs.md#adr-007-scalar-api-reference-and-built-in-aspnet-core-openapi)).
- **Infrastructure layer**: persists aggregates with Entity Framework Core and MySQL.

---

## Technologies

- C# 14 and .NET 10 (`net10.0`)
- ASP.NET Core Web API (REST controllers + built-in OpenAPI generation)
- Scalar.AspNetCore for interactive API documentation
- Entity Framework Core 10 + MySQL provider (`MySql.EntityFrameworkCore`)
- Cortex.Mediator for domain-event publishing
- BCrypt.Net-Next for password hashing
- System.IdentityModel.Tokens.Jwt for JWT issuance and validation
- Localization with `.resx` resources (`en`, `es`)
- xUnit for aggregate unit tests
- PlantUML and Structurizr DSL (architecture diagrams in `docs/`)

---

## Features

- `IAM`: sign-in (`POST /api/v1/auth/login`), company registration (`POST /api/v1/auth/register`), password recovery (`POST /api/v1/auth/forgot-password`, `POST /api/v1/auth/reset-password`), signed-in user (`GET /api/v1/auth/me`), employees (`GET`/`POST /api/v1/employees`), technicians (`GET /api/v1/technicians`), profile and password (`PATCH /api/v1/users/{id}`, `PUT /api/v1/users/{id}/password`)
- `Companies`: locations (`GET`/`POST /api/v1/locations`, `PATCH /api/v1/locations/{id}`), equipment (`GET`/`POST /api/v1/equipments`), assignment (`PUT /api/v1/equipments/{id}/assign`), history (`GET /api/v1/equipments/{id}/history`)
- `Subscriptions`: plans (`GET /api/v1/plans`), balance (`GET /api/v1/subscriptions/balance`), subscribe and renew (`POST /api/v1/subscriptions`, `POST /api/v1/subscriptions/renew`), maintenance visits (`GET`/`POST /api/v1/maintenances`)
- `ServiceRequests`: tickets (`GET`/`POST /api/v1/tickets`), cancel, assign, start, and resolve (`PUT /api/v1/tickets/{id}/cancel|assign|start|diagnosis`)
- `Quotations`: quotes (`GET`/`POST /api/v1/quotes`), approval (`PUT /api/v1/quotes/{id}/status`), simulated payment (`POST /api/v1/quotes/{id}/pay`)
- `Notifications`: my notifications (`GET /api/v1/notifications`), mark as read (`PUT /api/v1/notifications/{id}/read`)
- RFC 9457 `ProblemDetails` for every error response (see [ADR-009](docs/adrs.md#adr-009-rfc-9457-problem-details-with-a-readable-message))
- Localized error messages, selected from the request's `Accept-Language` header (`en` default, `es` supported)

---

## Project Documentation

| Document | Description |
| :--- | :--- |
| [**Architecture Decision Records (ADRs)**](docs/adrs.md) | Architectural decisions (ADR-001 through ADR-012). |
| [**Study Guide**](docs/study-guide.md) | Explains the architecture, patterns, and framework features this codebase applies. |
| [**User Stories**](docs/user-stories.md) | Technical stories (TS-IAM, TS-COM, TS-SUB, TS-SR, TS-QUO, TS-NOT) for frontend developers integrating with the API. |
| [**Class Diagram**](docs/class-diagram.puml) | PlantUML class diagram of all bounded contexts. |
| [**Software Architecture**](docs/software-architecture.dsl) | Structurizr DSL C4 model (Context → Containers → Components). |
| [**Changelog**](CHANGELOG.md) | Version history and release notes. |
| [**License**](LICENSE.md) | Project licensing information (MIT). |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (or later)
- A reachable MySQL instance

### Restore dependencies

```bash
dotnet restore
```

### Run the API

```bash
dotnet run --project CompuCare.Platform
```

Open the interactive OpenAPI document at `/scalar/v1`.

### Run the tests

```bash
dotnet test
```
