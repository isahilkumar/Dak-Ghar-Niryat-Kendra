using Microsoft.AspNetCore.Mvc;
using DakGharNiryatKendra.Data;
using DakGharNiryatKendra.Models;
using DakGharNiryatKendra.Models.ViewModels;
using DakGharNiryatKendra.Services;
using Microsoft.EntityFrameworkCore;

namespace DakGharNiryatKendra.Controllers
{
    public class PostalStaffController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConsignmentService _consignmentService;
        private readonly IQrBarcodeService _qrBarcodeService;
        private readonly IAuthService _authService;

        public PostalStaffController(
            ApplicationDbContext context,
            IConsignmentService consignmentService,
            IQrBarcodeService qrBarcodeService,
            IAuthService authService)
        {
            _context = context;
            _consignmentService = consignmentService;
            _qrBarcodeService = qrBarcodeService;
            _authService = authService;
        }

        private async Task<User> GetActiveStaffAsync()
        {
            var user = await _authService.GetCurrentUserAsync();
            if (user == null || user.Role != UserRole.PostalStaff)
            {
                var defaultStaff = await _context.Users
                    .Include(u => u.AssignedDnkCenter)
                    .FirstOrDefaultAsync(u => u.Role == UserRole.PostalStaff);

                return defaultStaff ?? new User { Id = 4, FullName = "Ramesh Sharma (Postal Assistant)", Role = UserRole.PostalStaff };
            }
            return user;
        }

        public async Task<IActionResult> Dashboard()
        {
            var staff = await GetActiveStaffAsync();
            var center = staff.AssignedDnkCenter ?? await _context.DnkCenters.FirstOrDefaultAsync()
                ?? new DnkCenter { Id = 1, CenterCode = "DNK-DEL-01", CenterName = "New Delhi GPO DNK" };

            var pendingInductions = await _consignmentService.GetPendingInductionsAsync(center.Id);
            var today = DateTime.UtcNow.Date;
            var inductedToday = await _context.ExportConsignments
                .Include(c => c.Exporter)
                .Where(c => c.DnkCenterId == center.Id && c.InductedAt >= today)
                .ToListAsync();

            var readyForDispatch = await _context.ExportConsignments
                .Where(c => c.DnkCenterId == center.Id && c.Status == ConsignmentStatus.InductedAtDNK && c.DispatchManifestId == null)
                .ToListAsync();

            var todayPickups = await _context.PickupRequests
                .Include(p => p.ExportConsignment)
                .Include(p => p.Exporter)
                .Where(p => p.Status != PickupStatus.Completed)
                .ToListAsync();

            var manifests = await _consignmentService.GetRecentManifestsAsync();

            var model = new PostalStaffDashboardViewModel
            {
                StaffUser = staff,
                DnkCenter = center,
                TodayInductions = inductedToday.Count,
                PendingInductions = pendingInductions.Count,
                ReadyForDispatchCount = readyForDispatch.Count,
                ActivePickupsCount = todayPickups.Count,
                PendingInductionList = pendingInductions.Take(8).ToList(),
                InductedTodayList = inductedToday.Take(8).ToList(),
                RecentManifests = manifests.Take(5).ToList(),
                TodayPickups = todayPickups.Take(5).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Induction(string? trackingNumber)
        {
            ViewBag.ScannedNumber = trackingNumber;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PerformInduction(string trackingNumber, decimal verifiedWeightKg)
        {
            var staff = await GetActiveStaffAsync();
            var (success, message, consignment) = await _consignmentService.InductConsignmentAsync(trackingNumber, verifiedWeightKg, staff.FullName);

            if (success)
            {
                TempData["SuccessMessage"] = message;
                return RedirectToAction(nameof(InductionSuccess), new { id = consignment!.Id });
            }

            TempData["ErrorMessage"] = message;
            return RedirectToAction(nameof(Induction), new { trackingNumber });
        }

        public async Task<IActionResult> InductionSuccess(int id)
        {
            var consignment = await _consignmentService.GetConsignmentByIdAsync(id);
            if (consignment == null) return NotFound();

            ViewBag.BarcodeSvg = _qrBarcodeService.GenerateBarcode128Svg(consignment.TrackingNumber, 45);
            return View(consignment);
        }

        [HttpGet]
        public async Task<IActionResult> CreateManifest()
        {
            var staff = await GetActiveStaffAsync();
            var center = staff.AssignedDnkCenter ?? await _context.DnkCenters.FirstOrDefaultAsync();

            var readyArticles = await _context.ExportConsignments
                .Include(c => c.Exporter)
                .Where(c => (center == null || c.DnkCenterId == center.Id) && c.Status == ConsignmentStatus.InductedAtDNK && c.DispatchManifestId == null)
                .ToListAsync();

            ViewBag.Staff = staff;
            ViewBag.Center = center;
            return View(readyArticles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateManifest(int originCenterId, string destinationFpo, List<int> selectedConsignmentIds)
        {
            var staff = await GetActiveStaffAsync();

            if (selectedConsignmentIds == null || selectedConsignmentIds.Count == 0)
            {
                TempData["ErrorMessage"] = "Please select at least one inducted consignment to include in the dispatch bag.";
                return RedirectToAction(nameof(CreateManifest));
            }

            var manifest = await _consignmentService.CreateDispatchManifestAsync(originCenterId, destinationFpo, selectedConsignmentIds, staff.FullName);
            TempData["SuccessMessage"] = $"Dispatch Manifest {manifest.ManifestNumber} generated successfully with {manifest.TotalArticles} articles.";

            return RedirectToAction(nameof(PrintManifest), new { id = manifest.Id });
        }

        public async Task<IActionResult> PrintManifest(int id)
        {
            var manifest = await _context.DispatchManifests
                .Include(m => m.OriginDnkCenter)
                .Include(m => m.Consignments)
                    .ThenInclude(c => c.Exporter)
                .Include(m => m.Consignments)
                    .ThenInclude(c => c.Items)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (manifest == null) return NotFound();

            ViewBag.BagBarcodeSvg = _qrBarcodeService.GenerateBarcode128Svg(manifest.BagBarcodeNumber, 45);
            ViewBag.QrCodeBase64 = _qrBarcodeService.GenerateQrCodeBase64($"MNF:{manifest.ManifestNumber}|BAG:{manifest.BagBarcodeNumber}|ARTICLES:{manifest.TotalArticles}|WT:{manifest.TotalWeightKg}KG");

            return View(manifest);
        }

        [HttpPost]
        public async Task<IActionResult> AssignPickup(int pickupId, string postmanName, string postmanPhone)
        {
            var pickup = await _context.PickupRequests.FindAsync(pickupId);
            if (pickup != null)
            {
                pickup.Status = PickupStatus.Assigned;
                pickup.AssignedPostmanName = postmanName;
                pickup.AssignedPostmanPhone = postmanPhone;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Pickup #{pickup.Id} assigned to {postmanName} ({postmanPhone}).";
            }
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        public async Task<IActionResult> CompletePickup(int pickupId)
        {
            var pickup = await _context.PickupRequests
                .Include(p => p.ExportConsignment)
                .FirstOrDefaultAsync(p => p.Id == pickupId);

            if (pickup != null)
            {
                pickup.Status = PickupStatus.Completed;
                if (pickup.ExportConsignment != null)
                {
                    pickup.ExportConsignment.Status = ConsignmentStatus.PickedUp;
                    pickup.ExportConsignment.PickupStatus = PickupStatus.Completed;
                }
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Pickup #{pickup.Id} marked as completed. Article collected from exporter.";
            }
            return RedirectToAction(nameof(Dashboard));
        }
    }
}
