using Microsoft.AspNetCore.Mvc;
using DakGharNiryatKendra.Data;
using DakGharNiryatKendra.Models;
using DakGharNiryatKendra.Models.ViewModels;
using DakGharNiryatKendra.Services;
using Microsoft.EntityFrameworkCore;

namespace DakGharNiryatKendra.Controllers
{
    public class ExporterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConsignmentService _consignmentService;
        private readonly ITariffCalculatorService _tariffService;
        private readonly IQrBarcodeService _qrBarcodeService;
        private readonly IAuthService _authService;

        public ExporterController(
            ApplicationDbContext context,
            IConsignmentService consignmentService,
            ITariffCalculatorService tariffService,
            IQrBarcodeService qrBarcodeService,
            IAuthService authService)
        {
            _context = context;
            _consignmentService = consignmentService;
            _tariffService = tariffService;
            _qrBarcodeService = qrBarcodeService;
            _authService = authService;
        }

        private async Task<User> GetActiveExporterAsync()
        {
            var user = await _authService.GetCurrentUserAsync();
            if (user == null || user.Role != UserRole.Exporter)
            {
                // Fallback to primary exporter for demo
                var defaultExp = await _context.Users.FirstOrDefaultAsync(u => u.Role == UserRole.Exporter);
                return defaultExp ?? new User { Id = 1, FullName = "Demo Exporter", BusinessName = "Demo Export Co." };
            }
            return user;
        }

        public async Task<IActionResult> Dashboard()
        {
            var exporter = await GetActiveExporterAsync();
            var consignments = await _consignmentService.GetExporterConsignmentsAsync(exporter.Id);
            var pickups = await _context.PickupRequests
                .Include(p => p.ExportConsignment)
                .Where(p => p.ExporterId == exporter.Id)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var model = new ExporterDashboardViewModel
            {
                Exporter = exporter,
                TotalShipments = consignments.Count,
                ActiveShipments = consignments.Count(c => c.Status != ConsignmentStatus.Delivered && c.Status != ConsignmentStatus.ReturnedToOrigin),
                InTransitShipments = consignments.Count(c => c.Status >= ConsignmentStatus.DepartedOriginCountry && c.Status < ConsignmentStatus.Delivered),
                CustomsClearedShipments = consignments.Count(c => c.CustomsStatus == CustomsStatus.Approved),
                DeliveredShipments = consignments.Count(c => c.Status == ConsignmentStatus.Delivered),
                TotalExportValueINR = consignments.Sum(c => c.DeclaredValueINR),
                TotalExportValueUSD = Math.Round(consignments.Sum(c => c.DeclaredValueINR) / 86.50m, 2),
                RecentConsignments = consignments.Take(10).ToList(),
                PendingPickups = pickups.Take(5).ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> Shipments(string? status)
        {
            var exporter = await GetActiveExporterAsync();
            var query = _context.ExportConsignments
                .Include(c => c.DnkCenter)
                .Include(c => c.Items)
                .Where(c => c.ExporterId == exporter.Id);

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ConsignmentStatus>(status, out var parsedStatus))
            {
                query = query.Where(c => c.Status == parsedStatus);
                ViewBag.CurrentFilter = status;
            }

            var shipments = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
            ViewBag.Exporter = exporter;

            return View(shipments);
        }

        [HttpGet]
        public async Task<IActionResult> Book()
        {
            var exporter = await GetActiveExporterAsync();
            var centers = await _context.DnkCenters.Where(c => c.IsActive).OrderBy(c => c.State).ThenBy(c => c.City).ToListAsync();
            var countries = await _context.TariffZones.Select(t => new { t.CountryCode, t.CountryName }).Distinct().OrderBy(c => c.CountryName).ToListAsync();
            var hsCodes = await _context.HsCodeDirectories.Take(20).ToListAsync();

            ViewBag.Centers = centers;
            ViewBag.Countries = countries;
            ViewBag.HsCodes = hsCodes;
            ViewBag.Exporter = exporter;

            var model = new BookConsignmentViewModel
            {
                DnkCenterId = centers.FirstOrDefault()?.Id ?? 1,
                LutNumber = exporter.LutNumber,
                DestinationCountryCode = "US",
                RecipientCountry = "United States"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(BookConsignmentViewModel model, string? itemsJson)
        {
            var exporter = await GetActiveExporterAsync();

            if (!ModelState.IsValid)
            {
                ViewBag.Centers = await _context.DnkCenters.Where(c => c.IsActive).OrderBy(c => c.State).ToListAsync();
                ViewBag.Countries = await _context.TariffZones.Select(t => new { t.CountryCode, t.CountryName }).Distinct().OrderBy(c => c.CountryName).ToListAsync();
                ViewBag.HsCodes = await _context.HsCodeDirectories.Take(20).ToListAsync();
                ViewBag.Exporter = exporter;
                return View(model);
            }

            try
            {
                // Parse dynamic line items if provided
                if (!string.IsNullOrEmpty(itemsJson))
                {
                    var items = System.Text.Json.JsonSerializer.Deserialize<List<ConsignmentItemInputModel>>(itemsJson);
                    if (items != null && items.Count > 0)
                    {
                        model.Items = items;
                    }
                }

                var consignment = await _consignmentService.BookConsignmentAsync(model, exporter.Id);
                TempData["SuccessMessage"] = $"Export booking completed successfully! Article Tracking Number: {consignment.TrackingNumber}, PBE: {consignment.PbeNumber}";

                return RedirectToAction(nameof(Details), new { id = consignment.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to create booking: {ex.Message}");
                ViewBag.Centers = await _context.DnkCenters.Where(c => c.IsActive).OrderBy(c => c.State).ToListAsync();
                ViewBag.Countries = await _context.TariffZones.Select(t => new { t.CountryCode, t.CountryName }).Distinct().OrderBy(c => c.CountryName).ToListAsync();
                ViewBag.HsCodes = await _context.HsCodeDirectories.Take(20).ToListAsync();
                ViewBag.Exporter = exporter;
                return View(model);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var consignment = await _consignmentService.GetConsignmentByIdAsync(id);
            if (consignment == null)
            {
                return NotFound();
            }

            ViewBag.QrCodeBase64 = _qrBarcodeService.GenerateQrCodeBase64($"https://dnk.indiapost.gov.in/track/{consignment.TrackingNumber}");
            ViewBag.BarcodeSvg = _qrBarcodeService.GenerateBarcode128Svg(consignment.TrackingNumber, 45);

            return View(consignment);
        }

        public async Task<IActionResult> PrintPbe(int id)
        {
            var consignment = await _consignmentService.GetConsignmentByIdAsync(id);
            if (consignment == null) return NotFound();

            ViewBag.QrCodeBase64 = _qrBarcodeService.GenerateQrCodeBase64($"PBE:{consignment.PbeNumber}|TRK:{consignment.TrackingNumber}|IEC:{consignment.Exporter?.IecNumber}|VAL:{consignment.DeclaredValueINR}");
            ViewBag.BarcodeSvg = _qrBarcodeService.GenerateBarcode128Svg(consignment.TrackingNumber, 40);
            ViewBag.PbeBarcodeSvg = _qrBarcodeService.GenerateBarcode128Svg(consignment.PbeNumber.Replace("/", "").Replace("-", ""), 35);

            return View(consignment);
        }

        public async Task<IActionResult> PrintLabel(int id)
        {
            var consignment = await _consignmentService.GetConsignmentByIdAsync(id);
            if (consignment == null) return NotFound();

            ViewBag.QrCodeBase64 = _qrBarcodeService.GenerateQrCodeBase64($"UPU:S10|{consignment.TrackingNumber}|{consignment.RecipientPostalCode}|{consignment.DestinationCountryCode}");
            ViewBag.BarcodeSvg = _qrBarcodeService.GenerateBarcode128Svg(consignment.TrackingNumber, 55);

            return View(consignment);
        }

        public async Task<IActionResult> PrintInvoice(int id)
        {
            var consignment = await _consignmentService.GetConsignmentByIdAsync(id);
            if (consignment == null) return NotFound();

            ViewBag.QrCodeBase64 = _qrBarcodeService.GenerateQrCodeBase64($"INV:{consignment.InvoiceNumber}|EXP:{consignment.Exporter?.BusinessName}|VAL:{consignment.DeclaredValueINR}");

            return View(consignment);
        }

        [HttpPost]
        public async Task<IActionResult> RequestPickup(int consignmentId, DateTime scheduledDate, TimeSlot timeSlot, string? instructions)
        {
            var exporter = await GetActiveExporterAsync();
            var (success, message) = await _consignmentService.SchedulePickupAsync(consignmentId, exporter.Id, scheduledDate, timeSlot, instructions);

            if (success)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(nameof(Details), new { id = consignmentId });
        }
    }
}
