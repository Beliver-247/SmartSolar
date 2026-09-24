# Smart Solar Microgrid Trading System - Web Application

This repository contains the React web application for the Smart Solar Microgrid Trading System. 
It serves as the administration portal for the **Backoffice** and **Grid Operator** roles.

## Project Architecture
This is a standard React Single Page Application (SPA) built with Vite. It strictly acts as the presentation layer, relying entirely on the ASP.NET Core backend for all business logic, role-based authorization rules, and data persistence (MongoDB).

- **Framework:** React + Vite
- **Routing:** React Router v6
- **Styling:** Tailwind CSS
- **HTTP Client:** Axios (configured with interceptors for JWT injection)

## Setup Instructions

1. **Install dependencies:**
   ```bash
   npm install
   ```

2. **Configure Environment:**
   Ensure you have a `.env.development` file in the root directory:
   ```env
   VITE_API_BASE_URL=http://localhost:5088
   ```

3. **Run the development server:**
   ```bash
   npm run dev
   ```

4. **Run Tests:**
   ```bash
   npx vitest run
   ```

## Roles and Scope
There is **no prosumer login** in this application. Prosumer operations (QR scanning, transfers, viewing bookings) are handled exclusively by the Android mobile app.

### Backoffice Role
- Manage web users (Create Backoffice/Operator accounts).
- Prosumer profile management (View, Edit, Deactivate, Reactivate).
- Manage Microgrid Nodes/Stations (Create, Edit capacity, Deactivate).

### Grid Operator Role
- Manage Stations (Update available battery slots).
- Monitor and manage reservations on behalf of prosumers.

## Backend Dependencies & CORS
> **CRITICAL:** Ensure the ASP.NET Core backend is running before using this UI. The UI will hit `http://localhost:5088` by default. 
> The backend has been configured to allow CORS requests from `http://localhost:5173`. If you run this UI on a different port, you must update the backend CORS policy.

## Consumed Endpoints
- `POST /api/auth/login`
- `GET/POST /api/users`
- `GET/PUT /api/prosumers`
- `GET/POST/PUT/DELETE /api/stations`
- `GET/POST/PUT/DELETE /api/reservations`
