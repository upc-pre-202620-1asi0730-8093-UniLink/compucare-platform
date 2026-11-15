# CompuCare Platform — REST API Technical Stories

## Overview

Technical stories for frontend developers integrating with the CompuCare Platform REST API (ASP.NET Core / .NET 10).

Common conventions:
- Base path: `/api/v1`
- Content-Type: `application/json`; JSON names are camelCase and every identifier is an integer.
- Error responses follow RFC 9457 `ProblemDetails` (`type`, `title`, `status`, `detail`, `instance`) plus `message`, localized by `Accept-Language` (`en` default, `es`).
- Authentication: `Authorization: Bearer <token>`, required everywhere except sign-in, registration, password recovery, and plans.
- Roles: `sysadmin`, `technician`, `admin`, `company_manager`, `employee`. A role outside the allowed list receives `403 Forbidden`.

---

## Bounded Context: IAM

### TS-IAM-001 — Sign In
As a frontend developer, I want to exchange credentials for a token so that I can open the dashboard of each role.

- `POST /api/v1/auth/login` with `email`, `password` → `200 OK` with `{ token, tokenType, user }`, where `user` is `{ id, companyId, name, email, role, phone, position }`.
- Wrong credentials or a disabled account → `401 Unauthorized`.

### TS-IAM-002 — Register a Company
As a frontend developer, I want to register a company with its first administrator so that a new customer can start using CompuCare.

- `POST /api/v1/auth/register` with `{ company: { name, ruc }, admin: { name, email, password }, planId?, payment?: { holder, last4 } }` → `201 Created` with `{ user, company }`.
- A RUC without 11 digits, a missing field, or a password shorter than six characters → `400 Bad Request`.
- A registered RUC or email → `409 Conflict`. An unknown `planId` → `404 Not Found`.

### TS-IAM-003 — Recover a Password
As a frontend developer, I want to request and use a reset token so that users can recover their account.

- `POST /api/v1/auth/forgot-password` with `email` → `200 OK` with `{ message, devToken }`; the response is the same whether or not the account exists, and `devToken` is only filled in `Development`.
- `POST /api/v1/auth/reset-password` with `token`, `password` → `200 OK`, or `400 Bad Request` for an invalid, used, or expired token.

### TS-IAM-004 — Manage Employees
As a frontend developer, I want to list and invite employees so that company administrators can assign devices to them.

- `GET /api/v1/employees` → `200 OK` with the employees of the signed-in administrator's company.
- `POST /api/v1/employees` with `name`, `email`, `position?` → `201 Created` with the employee and `tempPassword` (only in `Development`); a registered email → `409 Conflict`.

### TS-IAM-005 — Update My Account
As a frontend developer, I want to edit the profile and password of the signed-in user.

- `GET /api/v1/auth/me` → `200 OK` with the signed-in user.
- `PATCH /api/v1/users/{id}` with `name`, `phone?` → `200 OK`; another user's id → `403 Forbidden`.
- `PUT /api/v1/users/{id}/password` with `currentPassword`, `newPassword` → `200 OK`, or `400 Bad Request` when the current password does not match.

### TS-IAM-006 — List Technicians
As a frontend developer, I want the active technicians so that UniLink support can assign tickets.

- `GET /api/v1/technicians` (`sysadmin`) → `200 OK` with the active technicians.

---

## Bounded Context: Companies

### TS-COM-001 — Manage Locations
As a frontend developer, I want to manage the sites and desks of a company so that devices can be located.

- `GET /api/v1/locations` → `200 OK` with the locations of my company.
- `POST /api/v1/locations` with `name`, `desk` (`admin`, `company_manager`) → `201 Created`; a repeated site and desk → `409 Conflict`.
- `PATCH /api/v1/locations/{id}` with `active?`, `name?`, `desk?` → `200 OK`, or `404 Not Found` outside my company.

### TS-COM-002 — Register and List Equipment
As a frontend developer, I want to register and filter devices so that I can show the inventory.

- `GET /api/v1/equipments?status&type&locationId&q` → `200 OK`; employees only receive the devices assigned to them. `status` is `Operativo` or `En Reparación`.
- `POST /api/v1/equipments` with `name`, `serialNumber`, `type`, `locationId?` → `201 Created` with `{ id, companyId, code, name, serialNumber, type, locationId, assignedTo, status }`.
- A repeated serial number → `409 Conflict`; an inactive or foreign location → `400 Bad Request`.

### TS-COM-003 — Assign Equipment and Read Its History
As a frontend developer, I want to assign a device and show its history.

- `PUT /api/v1/equipments/{id}/assign` with `employeeId` (or `null` to unassign) → `200 OK`; an employee of another company → `400 Bad Request`.
- `GET /api/v1/equipments/{id}/history` → `200 OK` with `{ equipment, assignments, tickets, maintenances }`; each ticket includes its `quotes`.

---

## Bounded Context: Subscriptions

### TS-SUB-001 — Browse Plans
As a frontend developer, I want the plan catalog so that I can show prices on the landing page and on registration.

- `GET /api/v1/plans` (public) → `200 OK` with `{ id, code, name, price, hours, preventiveVisits }`, cheapest first.

### TS-SUB-002 — Subscription Balance
As a frontend developer, I want the coverage balance of my company.

- `GET /api/v1/subscriptions/balance` → `200 OK` with `hoursTotal`, `hoursUsed`, `hoursRemaining`, `percentUsed`, `preventiveTotal`, `preventiveUsed`, `preventiveRemaining`, `startedAt`, `renewsAt`; `404 Not Found` without a subscription.
- `POST /api/v1/subscriptions` with `planId`, `billing: { name, email }` → `201 Created` with the balance.
- `POST /api/v1/subscriptions/renew` → `200 OK` with a new period of the current plan.

### TS-SUB-003 — Schedule Preventive Maintenance
As a frontend developer, I want to book preventive visits for some devices.

- `GET /api/v1/maintenances` → `200 OK` with `{ id, companyId, date, time, equipmentIds, status }`, latest first.
- `POST /api/v1/maintenances` with `date` (`yyyy-MM-dd`), `time`, `equipmentIds` → `201 Created`.
- A past date, duplicated devices, or devices of another company → `400 Bad Request`; no visits left or an expired period → `409 Conflict`.

---

## Bounded Context: Service Requests

### TS-SR-001 — Report an Issue
As a frontend developer, I want employees to report a problem with one of their devices.

- `POST /api/v1/tickets` with `equipmentId`, `category`, `description` (`employee`) → `201 Created` with the ticket and status `Pendiente de Asignación`.
- A device not assigned to the employee → `403 Forbidden`; a description shorter than ten characters → `400 Bad Request`.

### TS-SR-002 — List Tickets
As a frontend developer, I want each role to see its own tickets.

- `GET /api/v1/tickets` → `200 OK`. Employees see their tickets, technicians the assigned ones, administrators their company's, and `sysadmin` all of them. Each ticket includes `equipmentName`, `employeeName`, `technicianName`, and a `timeline`.

### TS-SR-003 — Work a Ticket
As a frontend developer, I want to move a ticket through its lifecycle.

- `PUT /api/v1/tickets/{id}/cancel` (requester, before assignment) → `200 OK`, status `Cancelado`.
- `PUT /api/v1/tickets/{id}/assign` with `technicianId` (`sysadmin`) → `200 OK`, status `Asignado`; the technician is notified.
- `PUT /api/v1/tickets/{id}/start` (assigned technician) → `200 OK`, status `En Diagnóstico`.
- `PUT /api/v1/tickets/{id}/diagnosis` with `diagnosis`, `workDone`, `hours` (assigned technician) → `200 OK`, status `Atendido`; the hours are consumed from the company subscription.
- An operation not allowed in the current status, a pending or rejected quotation, already registered hours, or insufficient hours → `409 Conflict`.

---

## Bounded Context: Quotations

### TS-QUO-001 — Issue a Quotation
As a frontend developer, I want technicians to quote extra parts or work.

- `POST /api/v1/quotes` with `ticketId`, `items: [{ description, cost }]` (assigned technician) → `201 Created` with status `PENDING`; the company managers are notified.
- A ticket that is not assigned, in diagnosis, or is paused → `409 Conflict`; 0 or more than 50 items, or a cost with more than two decimals → `400 Bad Request`.

### TS-QUO-002 — Approve, Reject, and Pay
As a frontend developer, I want company administrators to decide on quotations.

- `GET /api/v1/quotes` → `200 OK`; administrators see their company's, technicians their own, `sysadmin` all of them.
- `PUT /api/v1/quotes/{id}/status` with `status: "APPROVED" | "REJECTED"` → `200 OK`. Rejecting pauses the ticket; approving resumes it when no other quotation is rejected.
- `POST /api/v1/quotes/{id}/pay` with `holder`, `last4` → `200 OK` with status `PAID` and a `receipt`. The payment is simulated and no card data is stored.

---

## Bounded Context: Notifications

### TS-NOT-001 — Read My Notifications
As a frontend developer, I want to show in-app notifications.

- `GET /api/v1/notifications` → `200 OK` with `{ id, userId, message, createdAt, read }`, latest first.
- `PUT /api/v1/notifications/{id}/read` → `200 OK`, or `404 Not Found` for another user's notification.
