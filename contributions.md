# SmartSolar Microgrid Trading System - Team Contributions

### 🔗 Project Links
- **YouTube Demonstration Video:** `[INSERT YOUTUBE LINK HERE]`
- **GitHub Repository:** `[INSERT GITHUB REPO LINK HERE]`

---

## 👥 Team Members & Contributions

### 1. Mendis J.D.L. (IT22110084)
**Feature Focus:** Authentication & Web User Management
* **Backend (.NET):** Developed the JWT authentication service, User repository, and Controllers for Backoffice/GridOperator web users. Configured security middleware and app settings.
* **Frontend (React):** Implemented the global `AuthContext`, login page, staff management directory, and protected routing wrappers.
* **Mobile (Android):** Built the login/registration flows, client-side validation rules (NIC, phone, password), and the Retrofit `AuthInterceptor` for secure token management.

### 2. Rathnayaka D.M.B.H. (IT22149930)
**Feature Focus:** Prosumer Management & Shared Infrastructure
* **Backend (.NET):** Handled prosumer profile management (CRUD & activation). Configured shared infrastructure such as MongoDB connection strings, CORS, `Program.cs` startup, and the global exception handling middleware.
* **Frontend (React):** Configured the base Axios instances, global Tailwind/Vite build configurations, and built the Prosumer directory UI.
* **Mobile (Android):** Handled local offline caching with SQLite (`solargrid.db`), `SessionManager` preferences, and the Prosumer dashboard metrics.

### 3. Kodithuwakku I.P. (IT22189776)
**Feature Focus:** Station & Booking Slot Management
* **Backend (.NET):** Built the infrastructure for solar stations and energy booking slots, including GPS location models, capacity management, and nested scheduling repositories.
* **Frontend (React):** Developed the Station configuration UI and built the core reusable UI components across the web app (Buttons, Modals, Inputs, Loading Spinners).
* **Mobile (Android):** Implemented the interactive Google Maps integration with auto-fitting bounds, station catalog browsing, and offline-fallback station seeds with exact Sri Lankan GPS coordinates.

### 4. Thassara M.P.M. (IT22239198)
**Feature Focus:** Reservation Management & Energy Transfer (QR)
* **Backend (.NET):** Engineered the complex energy reservation lifecycle (Pending -> Approved -> Completed/Cancelled). Implemented the Transfer API and QR code validation logic.
* **Frontend (React):** Built the main Application Shell (Sidebar, Layout, AppRoutes) and the Reservation Approvals dashboard with real-time pending/approved tracking.
* **Mobile (Android):** Developed the Grid Operator dashboard, reservation filtering logic, camera-based Google ML Kit Barcode Scanning for QR Passes, and the energy transfer verification UI.
