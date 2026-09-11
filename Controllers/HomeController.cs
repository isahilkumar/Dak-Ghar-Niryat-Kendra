using Microsoft.AspNetCore.Mvc;
using DakGharNiryatKendra.Data;
using DakGharNiryatKendra.Models;
using DakGharNiryatKendra.Models.ViewModels;
using DakGharNiryatKendra.Services;
using Microsoft.EntityFrameworkCore;

namespace DakGharNiryatKendra.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITariffCalculatorService _tariffService;
        private readonly IAuthService _authService;

        public HomeController(
            ApplicationDbContext context,
            ITariffCalculatorService tariffService,
            IAuthService authService)
        {
            _context = context;
            _tariffService = tariffService;
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var totalCenters = await _context.DnkCenters.CountAsync();
                var totalConsignments = await _context.ExportConsignments.CountAsync();
                var totalValue = (await _context.ExportConsignments.Select(c => (decimal?)c.DeclaredValueINR).SumAsync()) ?? 0m;
                var activeUsers = await _context.Users.CountAsync(u => u.Role == UserRole.Exporter);

                ViewBag.TotalCenters = totalCenters;
                ViewBag.TotalConsignments = totalConsignments;
                ViewBag.TotalValueINR = totalValue;
                ViewBag.TotalExporters = activeUsers;

                ViewBag.FeaturedCenters = await _context.DnkCenters.OrderBy(c => c.State).ThenBy(c => c.City).ToListAsync();
                ViewBag.RecentShipments = await _context.ExportConsignments
                    .Include(c => c.DnkCenter)
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(5)
                    .ToListAsync();

                ViewBag.TopDestinations = await _context.TariffZones
                    .Select(t => new { t.CountryCode, t.CountryName, t.PostalZone, t.BaseRateINR, t.MinDeliveryDays, t.MaxDeliveryDays })
                    .Distinct()
                    .OrderBy(t => t.CountryName)
                    .ToListAsync();

                ViewBag.PopularHsCodes = await _context.HsCodeDirectories.OrderByDescending(h => h.RodtepRatePercent).ToListAsync();
                ViewBag.CurrentUser = await _authService.GetCurrentUserAsync();
            }
            catch (Exception ex)
            {
                ViewBag.TotalCenters = 8;
                ViewBag.TotalConsignments = 5;
                ViewBag.TotalValueINR = 1250000m;
                ViewBag.TotalExporters = 12;
                ViewBag.FeaturedCenters = new List<DnkCenter>();
                ViewBag.TopDestinations = new List<dynamic>();
                ViewBag.PopularHsCodes = new List<HsCodeDirectory>();
                ViewBag.ErrorMessage = ex.Message;
            }

            return View("Index");
        }

        [HttpGet("/Welcome")]
        [HttpGet("/Home/Welcome")]
        public async Task<IActionResult> Welcome()
        {
            return await Index();
        }

        [HttpGet]
        public async Task<IActionResult> Calculator(string? country = "US", decimal weight = 1.0m, decimal length = 25m, decimal width = 20m, decimal height = 10m, decimal value = 5000m)
        {
            var model = new RateCalculatorViewModel
            {
                DestinationCountryCode = country ?? "US",
                WeightKg = weight > 0 ? weight : 1.0m,
                LengthCm = length > 0 ? length : 25m,
                WidthCm = width > 0 ? width : 20m,
                HeightCm = height > 0 ? height : 10m,
                DeclaredValueINR = value > 0 ? value : 5000m,
                IncludeInsurance = true
            };

            model.CalculatedRates = await _tariffService.CalculateRatesAsync(
                model.DestinationCountryCode,
                model.WeightKg,
                model.LengthCm,
                model.WidthCm,
                model.HeightCm,
                model.DeclaredValueINR,
                model.IncludeInsurance);

            ViewBag.Countries = await _context.TariffZones
                .Select(t => new { t.CountryCode, t.CountryName })
                .Distinct()
                .OrderBy(t => t.CountryName)
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Locator(string? search, string? state)
        {
            var query = _context.DnkCenters.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(c => c.CenterName.ToLower().Contains(search)
                    || c.City.ToLower().Contains(search)
                    || c.Pincode.Contains(search)
                    || c.Address.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                query = query.Where(c => c.State.ToLower() == state.Trim().ToLower());
            }

            var centers = await query.OrderBy(c => c.State).ThenBy(c => c.City).ToListAsync();

            ViewBag.SearchTerm = search;
            ViewBag.SelectedState = state;
            ViewBag.States = await _context.DnkCenters.Select(c => c.State).Distinct().OrderBy(s => s).ToListAsync();

            return View(centers);
        }

        public IActionResult Guidelines()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
