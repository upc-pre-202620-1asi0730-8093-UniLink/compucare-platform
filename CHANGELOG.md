# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-11-15

### Added
- Shared bounded context: `AggregateRoot`, `Result<T>`/`Error`, `IBaseRepository<TEntity>`, domain event dispatch and audit-timestamp EF Core interceptors, RFC 9457 problem details, Scalar/OpenAPI.
- IAM bounded context: company registration with an optional plan, sign-in with JWT issuance, employees, technicians, profile and password updates, one-time password recovery tokens, and role-aware request authorization.
- Companies bounded context: companies, locations, and equipment, with an assignment history per device and a status derived from its open tickets.
- Subscriptions bounded context: the published plan catalog, company subscriptions with idempotent support-hour consumption, renewals, and preventive maintenance visits.
- ServiceRequests bounded context: the ticket lifecycle (report, assign, start diagnosis, resolve, cancel) with a status timeline and an 80% coverage alert.
- Quotations bounded context: itemized quotations, approval or rejection (pausing the ticket while a quotation is rejected), and a simulated payment receipt.
- Notifications bounded context: in-app notifications per user.
- Cross-context collaboration through anti-corruption-layer facades and published-language records.
- Localized error messages (`en`, `es`) and aggregate unit tests.
