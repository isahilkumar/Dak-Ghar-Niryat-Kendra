<div align="center">

<img src="https://upload.wikimedia.org/wikipedia/commons/thumb/0/05/Flag_of_India.svg/320px-Flag_of_India.svg.png" width="80" alt="India Flag"/>

# 🏣 Dak Ghar Niryat Kendra
### India Post · Export Portal (DNK)

[![.NET](https://img.shields.io/badge/.NET_10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET_Core_MVC-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://docs.microsoft.com/aspnet/core/mvc)
[![SQLite](https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite&logoColor=white)](https://sqlite.org/)
[![Bootstrap 5](https://img.shields.io/badge/Bootstrap_5.3-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)](https://getbootstrap.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)](LICENSE)

> **Empowering Indian MSMEs, artisans and e-commerce exporters to ship globally — 100% paperless, zero-rated IGST, automated customs clearance.**

[🚀 Live Demo](#-quick-start) · [📖 Docs](#-architecture-overview) · [🐛 Issues](https://github.com/isahilkumar/Dak-Ghar-Niryat-Kendra/issues) · [⭐ Star this repo](#)

---

</div>

## 📋 Table of Contents

- [✨ Overview](#-overview)
- [🎯 Key Features](#-key-features)
- [🏗️ Tech Stack](#️-tech-stack)
- [🗂️ Project Structure](#️-project-structure)
- [🚀 Quick Start](#-quick-start)
- [👥 Demo Roles & Portals](#-demo-roles--portals)
- [📦 Postal Services Supported](#-postal-services-supported)
- [🌍 Destination Countries](#-destination-countries)
- [🗄️ Database & Seeding](#️-database--seeding)
- [🎨 UI Highlights](#-ui-highlights)
- [🔐 Authentication](#-authentication)
- [🛤️ Roadmap](#️-roadmap)
- [🤝 Contributing](#-contributing)
- [📄 License](#-license)

---

## ✨ Overview

**Dak Ghar Niryat Kendra (DNK)** is a full-stack digital platform built on **ASP.NET Core 10 MVC** that models the real-world [India Post Export Portal](https://www.indiapost.gov.in). It enables:

- **MSME Exporters** to file electronic Postal Bills of Export (e-PBE) under Section 84 of the Customs Act, 1962
- **Postal Staff** to induct, weigh, and dispatch parcels through Foreign Post Offices (FPO)
- **Customs Officers** to scrutinize e-declarations and grant Out-of-Charge (OOC)
- **Administrators** to monitor analytics, manage tariff zones, and oversee the DNK Kendra network

All workflows are paperless — from S10 barcode generation to IGST-zero-rated export declarations.

---

## 🎯 Key Features

<table>
<tr>
<td width="50%">

### 📬 Exporter Portal
- ✅ Book export consignment (e-PBE)
- ✅ Dynamic line-item entry with HS Code lookup
- ✅ Auto-generated UPU S10 barcode label (printable)
- ✅ Real-time postage calculation by service & destination
- ✅ LUT (Letter of Undertaking) reference linkage for zero-rated IGST
- ✅ Schedule doorstep pickup by local postman
- ✅ RoDTEP & Duty Drawback tracking

</td>
<td width="50%">

### 🏢 Postal Staff Portal
- ✅ Counter induction with barcode scanning
- ✅ Parcel weight verification & volumetric calculation
- ✅ FPO dispatch bag creation & manifest generation
- ✅ Pickup assignment to beat postmen
- ✅ Status updates at each processing milestone

</td>
</tr>
<tr>
<td width="50%">

### 🛃 Customs & FPO Portal
- ✅ Electronic declaration scrutiny (Sec 84 Customs Act)
- ✅ Export duty assessment & OOC grant workflow
- ✅ HS Code compliance checks
- ✅ ICEGATE EDI gateway simulation

</td>
<td width="50%">

### 📊 Admin & Analytics Portal
- ✅ National performance metrics dashboard
- ✅ DNK Kendra network management (8+ locations)
- ✅ MSME exporter registry
- ✅ Tariff zone & zone rate management
- ✅ Dispatch manifest oversight

</td>
</tr>
</table>

### 🌐 Additional Features
- **🔍 Live Package Tracking** — 13-digit UPU S10 barcode lookup with animated flight radar banner
- **🧮 Postage Calculator** — Instant tariff estimation for EMS, Air Parcel, ITPS, and Registered Mail
- **🗺️ Kendra Locator** — Interactive map showing all DNK export offices across India
- **🌏 Multi-language Ready** — i18n support structure (English / हिन्दी)
- **📱 Fully Responsive** — Mobile-first design with Bootstrap 5.3

---

## 🏗️ Tech Stack

| Layer | Technology |
|---|---|
| **Framework** | ASP.NET Core 10.0 MVC |
| **ORM** | Entity Framework Core 10 |
| **Database** | SQLite (dev) / SQL Server (prod) |
| **Frontend** | Bootstrap 5.3, Vanilla CSS, Bootstrap Icons 1.11 |
| **Typography** | Outfit, Plus Jakarta Sans, JetBrains Mono (Google Fonts) |
| **Charts** | Chart.js 4.4 |
| **QR/Barcode** | QRCoder (server-side generation) |
| **PDF Labels** | iTextSharp / printable HTML |
| **Auth** | Session-based (demo, no ASP.NET Identity) |
| **Hosting Ready** | Azure App Service / Railway / Render |

---

## 🗂️ Project Structure

```
Dak Ghar Niryat Kendra/
├── Controllers/
│   ├── HomeController.cs          # Public homepage, calculator, locator
│   ├── ExporterController.cs      # Exporter: book, shipments, pickup
│   ├── StaffController.cs         # Counter induction, dispatch
│   ├── CustomsController.cs       # OOC scrutiny workflow
│   ├── AdminController.cs         # Analytics, kendra mgmt
│   ├── TrackingController.cs      # Public S10 tracking
│   └── ApiController.cs           # AJAX endpoints (rates, HS codes)
│
├── Models/
│   ├── ExportConsignment.cs        # Core shipment entity
│   ├── ConsignmentItem.cs          # Line-item goods (HS codes)
│   ├── TariffZone.cs               # Postage rates by country/service
│   ├── DnkCenter.cs                # Kendra office records
│   ├── TrackingMilestone.cs        # Tracking event log
│   ├── DispatchManifest.cs         # FPO bag dispatch
│   ├── PickupRequest.cs            # Postman pickup schedule
│   ├── User.cs                     # Multi-role user model
│   └── ViewModels/                 # Typed view models
│
├── Data/
│   ├── ApplicationDbContext.cs     # EF Core DbContext
│   └── DbInitializer.cs           # Seed data (centers, tariffs, demo users)
│
├── Services/
│   ├── ConsignmentService.cs       # Business logic: booking, PBE
│   ├── AuthService.cs              # Session-based auth
│   └── QrBarcodeService.cs         # UPU S10 barcode generation
│
├── Views/
│   ├── Home/                       # Index, Calculator, Locator
│   ├── Exporter/                   # Book, Shipments, Details, PrintLabel
│   ├── Staff/                      # Dashboard, Induction, Dispatch
│   ├── Customs/                    # Declaration, OOC workflow
│   ├── Admin/                      # Dashboard, Kendras, Analytics
│   ├── Tracking/                   # Public track page
│   └── Shared/
│       ├── _Layout.cshtml          # Master layout (navbar, persona bar)
│       └── _LanguageModal.cshtml   # i18n language switcher
│
├── wwwroot/
│   ├── css/site.css               # Premium design system (~2600 lines)
│   ├── js/site.js                 # Scroll animations, reveal effects
│   └── images/                    # Logos, hero images
│
├── appsettings.json               # DB provider, connection strings
├── Program.cs                     # App bootstrap, DI, EF seed
└── DakGharNiryatKendra.csproj     # .NET 10 project file
```

---

## 🚀 Quick Start

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Git

### 1. Clone the Repository
```bash
git clone https://github.com/isahilkumar/Dak-Ghar-Niryat-Kendra.git
cd "Dak-Ghar-Niryat-Kendra"
```

### 2. Run the Application
```bash
dotnet run
```

The application will:
- ✅ Restore NuGet packages automatically
- ✅ Create the SQLite database (`dnk_portal.db`)
- ✅ Seed all demo data (users, kendras, tariffs, consignments)
- ✅ Start the web server

### 3. Open in Browser
```
http://localhost:5169
```

### 🔄 Development Mode (Hot Reload)
```bash
dotnet watch
```

### 🗄️ Switch to SQL Server
Edit `appsettings.json`:
```json
{
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "SqlServerConnection": "Server=.;Database=DnkPortal;Trusted_Connection=True;"
  }
}
```
Then run:
```bash
dotnet ef database update
```

---

## 👥 Demo Roles & Portals

The application ships with pre-seeded demo accounts. Switch roles using the **top persona bar** on any page — no login required for the demo.

| Role | Username | Portal Access | Shortcut URL |
|------|----------|---------------|-------------|
| 🏪 **MSME Exporter** | `exporter1` | Book e-PBE, shipments, pickup | `/Account/SwitchUser?username=exporter1` |
| 📮 **DNK Counter Staff** | `postal_staff` | Induct, scan, dispatch bags | `/Account/SwitchUser?username=postal_staff` |
| 🛃 **Customs Officer** | `customs_officer` | OOC scrutiny & clearance | `/Account/SwitchUser?username=customs_officer` |
| ⚙️ **Admin / Director** | `admin` | Analytics & network oversight | `/Account/SwitchUser?username=admin` |

---

## 📦 Postal Services Supported

| Service | Max Weight | Transit Time | Best For |
|---------|-----------|--------------|---------|
| 🔴 **EMS International Speed Post** | 30 Kg | 3–7 Days | High-value, time-sensitive |
| 🟡 **International Air Parcel** | 20 Kg | 6–12 Days | Bulk commercial goods |
| 🔵 **ITPS Tracked Packet** | 2 Kg | 5–10 Days | Amazon, Etsy, eBay sellers |
| ⚪ **Registered Air Mail** | 2 Kg | 10–18 Days | Documents & light parcels |

---

## 🌍 Destination Countries

Tariff zones are seeded for **11 key export destinations** with full rate tables per service:

🇮🇳 India (SEZ/Domestic) · 🇺🇸 United States · 🇬🇧 United Kingdom · 🇦🇪 UAE · 🇩🇪 Germany · 🇦🇺 Australia · 🇨🇦 Canada · 🇸🇬 Singapore · 🇯🇵 Japan · 🇫🇷 France · 🌐 Others (Rest of World)

> The platform supports 210+ countries via the UPU postal network — additional tariff zones can be added via the Admin portal.

---

## 🗄️ Database & Seeding

The app uses **Entity Framework Core** with auto-migration and seed data:

```csharp
// Program.cs — runs on every startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Initialize(context);  // Seeds only if DB is empty
}
```

**Seeded Data includes:**
- 🏣 **8 DNK Centers** across India (Delhi, Mumbai, Jaipur, Kochi, Bengaluru, etc.)
- 👤 **4 Demo Users** (one per role)
- 📦 **5 Sample Export Consignments** with full tracking milestones
- 💰 **~26 Tariff Zone rows** (4 services × multiple countries)
- 🏷️ **50+ HS Code entries** for common MSME export goods
- 🚚 **1 Dispatch Manifest** and **1 Pickup Request**

---

## 🎨 UI Highlights

The frontend features a **premium dark glassmorphism design system** built entirely in Vanilla CSS:

- 🌑 **Dark Hero Banner** — Radial gradient starfield with animated grid mesh
- 💎 **Glass Card** — `backdrop-filter: blur(24px)` tracking widget with glow border
- ✈️ **Live Radar Banner** — Animated aircraft flight simulation with waypoint nodes
- 📊 **Animated Stats Bar** — Counter animation triggered by Intersection Observer
- 🎠 **Testimonial Marquee** — Infinite CSS marquee with dual-direction scroll tracks
- 🎯 **Tilt Cards** — CSS 3D perspective hover effects on portal cards
- 🌈 **Gradient Typography** — HSL-tuned tricolor gradient text for brand highlight
- 🔤 **Premium Fonts** — Outfit (headings) + Plus Jakarta Sans (body) + JetBrains Mono (codes)
- ♿ **Accessible** — ARIA labels, semantic HTML5, keyboard navigation

---

## 🔐 Authentication

This is a **demo portal** with session-based persona switching (no password required):

```csharp
// Switch any role instantly via URL
/Account/SwitchUser?username=exporter1
/Account/SwitchUser?username=postal_staff
/Account/SwitchUser?username=customs_officer
/Account/SwitchUser?username=admin
```

> For production deployment, replace with ASP.NET Core Identity + JWT or cookie auth.

---

## 🛤️ Roadmap

- [ ] 📧 Email notification on consignment status change
- [ ] 🗺️ Live Google Maps integration for Kendra Locator
- [ ] 📱 Progressive Web App (PWA) support
- [ ] 🧾 PDF generation for e-PBE and shipping labels (iTextSharp)
- [ ] 🔗 Real ICEGATE EDI API integration (CBIC sandbox)
- [ ] 📊 Export analytics with Chart.js time-series graphs
- [ ] 🌐 Full Hindi (हिन्दी) i18n localization
- [ ] 🐳 Docker Compose deployment configuration
- [ ] 🔒 ASP.NET Core Identity full auth flow

---

## 🤝 Contributing

Contributions are welcome! Here's how to get started:

```bash
# 1. Fork the repo and clone your fork
git clone https://github.com/<your-username>/Dak-Ghar-Niryat-Kendra.git

# 2. Create a feature branch
git checkout -b feature/your-feature-name

# 3. Make your changes and commit
git add .
git commit -m "feat: add your feature description"

# 4. Push and open a Pull Request
git push origin feature/your-feature-name
```

Please follow conventional commits (`feat:`, `fix:`, `docs:`, `style:`, `refactor:`).

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

<div align="center">

**Made with ❤️ for Indian MSMEs, Artisans & E-commerce Exporters**

[![India Post](https://img.shields.io/badge/India_Post-DC2626?style=flat-square&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==&logoColor=white)](https://www.indiapost.gov.in)
[![MSME](https://img.shields.io/badge/MSME_Export-FF6B00?style=flat-square)](https://msme.gov.in)
[![DGFT](https://img.shields.io/badge/DGFT_IEC-0047AB?style=flat-square)](https://www.dgft.gov.in)

*Dak Ghar Niryat Kendra — Exporting India's Heritage to the World* 🇮🇳

</div>
