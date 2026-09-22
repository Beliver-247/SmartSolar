# Smart Solar Microgrid Trading System - Backend API

## Project Overview
This project is the FAT-service backend for the Smart Solar Microgrid Trading System. It is built using ASP.NET Core 8 Web API and MongoDB. The system enforces business rules, handles role-based authorization, and processes reservations and transfers for microgrid nodes. Both Web and Android clients communicate with this API via REST/JSON.

## Technologies
- ASP.NET Core 8
- C#
- MongoDB (`MongoDB.Driver`)
- JWT Authentication
- BCrypt (`BCrypt.Net-Next`) for password hashing
- QRCoder for QR generation
- Swagger/OpenAPI
- FluentValidation
- xUnit & Moq & FluentAssertions

## Architecture
The system uses a layered FAT-service architecture:

```text
Web UI & Android UI
   │
   │ REST/JSON
   ▼
ASP.NET Core API (SmartSolarMicrogrid.Api)
   │
   ├── Controllers   (Thin, HTTP handling)
   ├── Services      (Business Logic & Validation)
   ├── Validators    (FluentValidation DTO validation)
   └── Repositories  (Data access only)
            │
            ▼
         MongoDB
```

## MongoDB Setup
1. Install and start MongoDB locally or use a MongoDB cluster (e.g., MongoDB Atlas).
2. The database will be created automatically upon connection.
3. Database configuration is stored in `appsettings.json`:
   ```json
   "MongoDb": {
     "ConnectionString": "mongodb://localhost:27017",
     "DatabaseName": "SmartSolarMicrogrid"
   }
   ```

## Environment Variables
In a production deployment, do not hardcode secrets. Override the following settings via environment variables:
- `MongoDb__ConnectionString`
- `MongoDb__DatabaseName`
- `Jwt__Key`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Cors__AllowedOrigins`

## Running Locally
1. Navigate to the solution folder.
2. Restore packages:
   ```bash
   dotnet restore
   ```
3. Build the solution:
   ```bash
   dotnet build
   ```
4. Run tests:
   ```bash
   dotnet test
   ```
5. Run the API:
   ```bash
   dotnet run --project src/SmartSolarMicrogrid.Api/SmartSolarMicrogrid.Api.csproj
   ```

## Swagger
When running locally in Development mode, navigate to `http://localhost:<port>/swagger` to view and test all endpoints.
Swagger is configured with JWT Bearer authentication. Use the "Authorize" button to inject your token.

## Authentication
To obtain a JWT:
1. `POST /api/auth/login` with `UsernameOrNic` and `Password`.
2. A default admin user is seeded:
   - **Username:** `admin`
   - **Password:** `admin123`
   - **Role:** `Backoffice`

## Roles
| Role | Permissions |
|---|---|
| **Backoffice** | Manage Users, Manage Prosumers (including Reactivation), Manage Stations. |
| **GridOperator** | View Stations, View/Create/Update/Cancel Reservations, Dashboard stats, Verify QR, Complete Transfers. |
| **Prosumer** | Register self, Edit own profile, Request deactivation, Create/Update/Cancel own reservations, Get own QR. |

## Business Rules Implemented
1. **7-day rule:** Energy slots must be reserved within 7 days of the booking request.
2. **12-hour rule:** Updates and cancellations require at least 12 hours' notice before the slot start time.
3. **Station Deactivation:** A station cannot be deleted/deactivated if it has `Pending` or `Approved` reservations.
4. **Prosumer Reactivation:** Deactivated prosumers can only be reactivated by the `Backoffice`.
5. **NIC Uniqueness:** Duplicate NIC registration is rejected, and NIC is immutable.
6. **QR Generation:** Generated only when a reservation is `Approved`.
7. **GridOperator Limitations:** Cannot manage users or stations; constrained by standard reservation time restrictions.
8. **Concurrency:** Atomic updates prevent double booking of the same slot.

## API Endpoints
- **Auth:** `/api/auth/login`, `/api/auth/register`
- **Users:** `/api/users` (CRUD)
- **Prosumers:** `/api/prosumers`, `/api/prosumers/{nic}` (CRUD, activate/deactivate)
- **Stations:** `/api/stations`, `/api/stations/{id}` (CRUD)
- **Booking Slots:** `/api/stations/{stationId}/slots`
- **Reservations:** `/api/reservations`, `/api/reservations/pending`, `/api/reservations/approved-future`, `/api/reservations/{id}/approve`, `/api/reservations/{id}/qr`
- **Transfers:** `/api/transfers/verify`, `/api/transfers/{reservationId}/complete`

## IIS Deployment
The application is configured for IIS Out-of-Process hosting via `web.config`.
1. Ensure the **.NET Core Hosting Bundle** is installed on the IIS server.
2. Publish the API:
   ```bash
   dotnet publish -c Release -o ./publish
   ```
3. Copy the contents of the `./publish` directory to your IIS site folder.
4. Set up an Application Pool configured to `No Managed Code`.
5. Configure site bindings and assign the folder permissions to `IIS_IUSRS`.
6. Use IIS Environment Variables to pass production secrets (e.g., JWT Key, MongoDB Connection String).

## Assumptions
- `GridOperator` approval of a reservation is handled through `PUT /api/reservations/{id}/approve`.
- Slot statuses are updated synchronously during reservation processes to ensure accurate capacity availability.
- Station Deactivation is implemented via HTTP DELETE logic but enforces the constraint requirement strictly.
