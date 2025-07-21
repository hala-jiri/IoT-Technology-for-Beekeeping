# IoT technology for Beekeeping
_ToDo-list with complete modules, upcoming tasks and known issues under description_

# About project

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

- [ ] ✅ Add unit tests for `BackupService`
- [ ] ✅ Test Restore from ZIP with various data
- [ ] ✅ Review all validation on input forms
- [ ] ✅ Add error logging and fallback where needed (e.g. try/catch in Azure Function)

---

## 🧠 Future Ideas

- [ ] Send email or webhook notification on backup error
- [ ] Store backups in Azure Blob Storage (optional)
- [ ] Monitor backup size / data growth over time
- [ ] Import/export Hive/Apiary configuration or Warehouse items
