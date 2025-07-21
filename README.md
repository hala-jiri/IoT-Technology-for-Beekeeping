# IoT technology for Beekeeping
_ToDo-list with complete modules, upcoming tasks and known issues under description_

## 📌 Project Description

BeeApp is a complete web-based system designed to help beekeepers manage their apiaries, hives, inspections, measurements, and equipment. It supports modern IoT integration — including live data from hive sensors — and provides powerful features like automatic backups, public hive sharing, and warehouse tracking.

This app was built with practicality in mind: designed for daily use by beekeepers, including solo hobbyists and small community apiaries.

The platform allows you to:

- Track apiaries and individual hives
- Record and visualize live data (weight, temperature, etc.)
- Perform and document inspections with standardized forms
- Export data for analysis or archiving
- Backup and restore the entire database
- Manage your beekeeping inventory
- Share selected hives with the public (without exposing sensitive data)
- Automate backups via Azure Function
- Prepare for multi-user support (login, ownership, visibility)

All key features are fully integrated with charts, filtering, export options, and designed to be simple and fast to use — even on mobile or tablets.

---

## ⚙️ Technologies Used

| Layer           | Tech                                                     |
|-----------------|----------------------------------------------------------|
| Web Frontend    | ASP.NET Core MVC (Razor Views)                           |
| API Backend     | ASP.NET Core Web API                                     |
| Database        | Entity Framework Core + SQL Server LocalDb (future Azure DB) |
| Data Format     | JSON (for IoT devices and exports)                       |
| Graphs          | Chart.js (via CDN, in Razor views)                       |
| Maps            | Leaflet.js + OpenStreetMap                               |
| IoT Input       | JSON POST (for ESP microcontrollers, etc.)              |
| Auth (Planned)  | ASP.NET Identity (multi-user mode, not yet implemented) |
| File Uploads    | ImageSharp (resizing + compression)                      |
| Scheduling      | Azure Function (Timer + HTTP Trigger)                    |
| Storage (ZIPs)  | Local `wwwroot/backups/` (Blob Storage planned)          |

---

## 🛠 Version Info

| Component                 | Version            |
|--------------------------|--------------------|
| .NET SDK                 | .NET 8.0           |
| ASP.NET Core MVC & WebAPI| 8.0                |
| Entity Framework Core    | 8.0                |
| Chart.js (CDN)           | 4.x (via unpkg)    |
| Leaflet.js (CDN)         | 1.9.x              |
| ImageSharp               | 3.x                |

---

## 👤 Author

Built by **Jiri (hala-jiri)**, beekeeper and developer.  
This project was created to combine the love for bees with the power of technology. 🐝❤️💻

---

## 💬 Contributions / Issues

If you have feedback, ideas, or bug reports, feel free to open an issue or contribute.

---

## 📄 License

This project is currently private. License and open-source status TBD.

&nbsp;  
&nbsp;  
&nbsp;

# 📝 TODO – Beekeeper App (BeeApp)

## ✅ Completed Modules / Features

- [x] Project setup (Web + API + Shared structure)
- [x] CRUD for Apiaries (including GPS, image, map)
- [x] CRUD for Hives (linked to Apiaries)
- [x] Measurement API for Hives + Apiaries (IoT JSON support)
- [x] Hive detail view – chart with aggregation, smoothing toggle
- [x] Dashboard: 24h chart preview for all hives with filters
- [x] Inspection module (form, list, edit, export)
- [x] Warehouse module (inventory tracking, status)
- [x] Data export to ZIP/JSON (with date range and hive selection)
- [x] Backup system (ZIP export + DB logging)
- [x] UI for backup overview + download button
- [x] Image and map displayed side-by-side
- [x] API endpoint `/api/backup` with API key validation
- [x] Serialization fixes (infinite loops, buffer size)
- [x] Input validation (GPS decimal handling, form clean-up)

---

## 🔜 Upcoming Tasks

### 🔧 Backup – automation & security
- [ ] Create Azure Function with timer trigger
- [ ] Function sends POST request to `/api/backup` with API key
- [ ] Deploy function to Azure (run daily or weekly)
- [ ] Store API key securely (Azure AppSetting or KeyVault)

### 🔁 Restore from ZIP (import)
- [ ] UI for uploading a ZIP file
- [ ] Unpack and read JSON contents
- [ ] Wipe current DB and insert data from backup
- [ ] Confirm import (security question/warning)
- [ ] Log import result and timestamp

### 👥 User management
- [ ] Add ASP.NET Core Identity (register/login)
- [ ] Link Apiaries and Hives to specific users
- [ ] Show only user-owned data in UI
- [ ] Admin role can access all data

### 🌐 Public Hive view
- [ ] Add `IsPublic` property to Hives
- [ ] Public shareable URL for each hive
- [ ] Hide GPS, owner, and apiary details in public view
- [ ] Optional embed-friendly readonly view

### 🪧 Milestones on charts
- [ ] Allow adding events to hives (date + comment)
- [ ] Display milestones as annotations on Hive charts
- [ ] CRUD support for managing milestones

---

## 🐞 Known Issues / Notes

- [ ] Backup API testing via Postman currently fails (possibly HTTPS/cert/port related)
- [ ] Large JSON serialization previously failed – fixed via DTOs and `IgnoreCycles`

---

## 🧪 Testing & Dev

- [ ] Add unit tests for `BackupService`
- [ ] Test Restore from ZIP with various data
- [ ] Review all validation on input forms
- [ ] Add error logging and fallback where needed (e.g. try/catch in Azure Function)

---

## 🧠 Future Ideas

- [ ] Send email or webhook notification on backup error
- [ ] Store backups in Azure Blob Storage (optional)
- [ ] Monitor backup size / data growth over time
- [ ] Import/export Hive/Apiary configuration or Warehouse items
