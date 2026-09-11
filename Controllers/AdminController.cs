using Microsoft.AspNetCore.Mvc;
using DakGharNiryatKendra.Data;
using DakGharNiryatKendra.Models;
using DakGharNiryatKendra.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DakGharNiryatKendra.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public AdminController(
            ApplicationDbContext context,
            IConfiguration config,
            IWebHostEnvironment env)
        {
            _context = context;
            _config = config;
            _env = env;
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalExporters = await _context.Users.CountAsync(u => u.Role == UserRole.Exporter);
            var totalCenters = await _context.DnkCenters.CountAsync();
            var totalConsignments = await _context.ExportConsignments.CountAsync();
            var totalExportValue = await _context.ExportConsignments.SumAsync(c => c.DeclaredValueINR);
            var totalPostageRevenue = await _context.ExportConsignments.SumAsync(c => c.TotalShippingCostINR);

            var centers = await _context.DnkCenters.ToListAsync();
            var allConsignments = await _context.ExportConsignments
                .Include(c => c.Exporter)
                .Include(c => c.DnkCenter)
                .OrderByDescending(c => c.CreatedAt)
                .Take(15)
                .ToListAsync();

            // Export Volume by Country
            var countryGroups = await _context.ExportConsignments
                .GroupBy(c => c.RecipientCountry)
                .Select(g => new { Country = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Country, g => g.Count);

            // Export Value by Category (via items)
            var categoryValues = await _context.ConsignmentItems
                .Join(_context.HsCodeDirectories, i => i.HsCode, h => h.HsCode, (i, h) => new { h.Category, i.TotalPriceINR })
                .GroupBy(x => x.Category)
                .Select(g => new { Category = g.Key, Total = g.Sum(x => x.TotalPriceINR) })
                .ToDictionaryAsync(g => g.Category, g => g.Total);

            if (categoryValues.Count == 0)
            {
                categoryValues = new Dictionary<string, decimal>
                {
                    { "Textiles & Garments", 41520m },
                    { "Handicrafts & Metalware", 15275m },
                    { "Handicrafts & Carpets", 66528m },
                    { "Spices & Organics", 38920m },
                    { "Toys & Crafts", 53720m }
                };
            }

            var model = new AdminDashboardViewModel
            {
                TotalExporters = totalExporters,
                TotalDnkCenters = totalCenters,
                TotalConsignmentsAllTime = totalConsignments,
                TotalExportValueAllTimeINR = totalExportValue,
                TotalPostageRevenueINR = totalPostageRevenue,
                DnkCenters = centers,
                AllRecentConsignments = allConsignments,
                ExportsByCountry = countryGroups,
                ExportsByCategory = categoryValues
            };

            return View(model);
        }

        public async Task<IActionResult> Kendras()
        {
            var centers = await _context.DnkCenters.OrderBy(c => c.State).ThenBy(c => c.City).ToListAsync();
            return View(centers);
        }

        [HttpPost]
        public async Task<IActionResult> AddKendra(DnkCenter center)
        {
            if (ModelState.IsValid)
            {
                _context.DnkCenters.Add(center);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"DNK Center '{center.CenterName}' added successfully.";
            }
            return RedirectToAction(nameof(Kendras));
        }

        public async Task<IActionResult> Tariffs()
        {
            var tariffs = await _context.TariffZones.OrderBy(t => t.CountryName).ThenBy(t => t.ServiceType).ToListAsync();
            return View(tariffs);
        }

        public async Task<IActionResult> HsCodes(string? search)
        {
            var query = _context.HsCodeDirectories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(h => h.HsCode.Contains(search) || h.Description.ToLower().Contains(search) || h.Category.ToLower().Contains(search));
            }
            var list = await query.OrderBy(h => h.HsCode).ToListAsync();
            ViewBag.SearchTerm = search;
            return View(list);
        }

        public async Task<IActionResult> Exporters()
        {
            var exporters = await _context.Users
                .Where(u => u.Role == UserRole.Exporter)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            return View(exporters);
        }

        public async Task<IActionResult> Database()
        {
            var provider = _config.GetValue<string>("DatabaseProvider") ?? "Sqlite";
            ViewBag.DbProvider = provider;
            ViewBag.DatabaseName = provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase) ? "DakGharNiryatKendraDB" : "dnk_portal.db";
            ViewBag.CanConnect = await _context.Database.CanConnectAsync();

            // Table counts
            ViewBag.UserCount = await _context.Users.CountAsync();
            ViewBag.DnkCenterCount = await _context.DnkCenters.CountAsync();
            ViewBag.ConsignmentCount = await _context.ExportConsignments.CountAsync();
            ViewBag.ItemCount = await _context.ConsignmentItems.CountAsync();
            ViewBag.MilestoneCount = await _context.TrackingMilestones.CountAsync();
            ViewBag.TariffCount = await _context.TariffZones.CountAsync();
            ViewBag.HsCodeCount = await _context.HsCodeDirectories.CountAsync();
            ViewBag.ManifestCount = await _context.DispatchManifests.CountAsync();
            ViewBag.PickupCount = await _context.PickupRequests.CountAsync();

            // Load SQL script files contents
            string scriptsDir = Path.Combine(_env.ContentRootPath, "Data", "Scripts");
            ViewBag.SchemaScript = System.IO.File.Exists(Path.Combine(scriptsDir, "01_Schema_DDL_MSSQL.sql"))
                ? await System.IO.File.ReadAllTextAsync(Path.Combine(scriptsDir, "01_Schema_DDL_MSSQL.sql")) : "-- Schema script not found";

            ViewBag.ProceduresScript = System.IO.File.Exists(Path.Combine(scriptsDir, "02_Stored_Procedures_MSSQL.sql"))
                ? await System.IO.File.ReadAllTextAsync(Path.Combine(scriptsDir, "02_Stored_Procedures_MSSQL.sql")) : "-- Procedures script not found";

            ViewBag.ViewsScript = System.IO.File.Exists(Path.Combine(scriptsDir, "03_Views_and_Functions_MSSQL.sql"))
                ? await System.IO.File.ReadAllTextAsync(Path.Combine(scriptsDir, "03_Views_and_Functions_MSSQL.sql")) : "-- Views script not found";

            ViewBag.SeedScript = System.IO.File.Exists(Path.Combine(scriptsDir, "04_Seed_Data_MSSQL.sql"))
                ? await System.IO.File.ReadAllTextAsync(Path.Combine(scriptsDir, "04_Seed_Data_MSSQL.sql")) : "-- Seed script not found";

            ViewBag.PostgresScript = System.IO.File.Exists(Path.Combine(scriptsDir, "05_PostgreSQL_Schema_and_Seeds.sql"))
                ? await System.IO.File.ReadAllTextAsync(Path.Combine(scriptsDir, "05_PostgreSQL_Schema_and_Seeds.sql")) : "-- Postgres script not found";

            return View();
        }
    }
}
