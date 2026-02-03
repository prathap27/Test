# Leave Management API

A simple C# backend API for managing leave requests with multi-level approvals and JWT-based authentication.

## Features
- JWT authentication and role-based authorization.
- Leave request submission for employees.
- Multi-level approvals configured via `appsettings.json`.
- Repository + service pattern with EF Core (in-memory provider).

## Quick Start
1. Ensure you have the .NET 8 SDK installed.
2. Run the API:

```bash
cd LeaveManagementApi
dotnet run
```

## Authentication
Use the preconfigured users in `appsettings.json`:
- `alice` / `Password123!` (Employee)
- `manager` / `Password123!` (Manager)
- `hr` / `Password123!` (HR)

Login:

```http
POST /api/auth/login
{
  "username": "alice",
  "password": "Password123!"
}
```

The response contains a bearer token to use in the `Authorization` header.

## API Endpoints
- `POST /api/leaves` (Employee) - Create a leave request.
- `GET /api/leaves/mine` (Authenticated) - Get your leave requests.
- `GET /api/leaves/pending-approvals` (Manager/HR) - View pending approvals.
- `POST /api/leaves/approvals/{approvalId}/decision` (Manager/HR) - Approve or reject.

### Approval Decision Payload
```json
{
  "decision": "Approved",
  "comment": "Approved for coverage"
}
```

## Configuration
The approval chain is configured in `appsettings.json` under `Auth:ApprovalLevels`. Each entry must map to a user role present in `Auth:Users`.
