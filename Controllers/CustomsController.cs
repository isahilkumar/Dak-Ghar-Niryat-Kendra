using Microsoft.AspNetCore.Mvc;
using DakGharNiryatKendra.Data;
using DakGharNiryatKendra.Models;
using DakGharNiryatKendra.Models.ViewModels;
using DakGharNiryatKendra.Services;
using Microsoft.EntityFrameworkCore;

namespace DakGharNiryatKendra.Controllers
{
    public class CustomsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConsignmentService _consignmentService;
        private readonly IQrBarcodeService _qrBarcodeService;
        private readonly IAuthService _authService;

        public CustomsController(
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

        private async Task<User> GetActiveOfficerAsync()
        {
            var user = await _authService.GetCurrentUserAsync();
            if (user == null || user.Role != UserRole.CustomsOfficer)
            {
                var defaultOfficer = await _context.Users
                    .FirstOrDefaultAsync(u => u.Role == UserRole.CustomsOfficer);

                return defaultOfficer ?? new User { Id = 5, FullName = "Vikramaditya Rathore, IRS", Designation = "Superintendent of Customs", Role = UserRole.CustomsOfficer };
            }
            return user;
        }

        public async Task<IActionResult> Dashboard()
        {
            var officer = await GetActiveOfficerAsync();
            var pendingScrutiny = await _consignmentService.GetPendingCustomsScrutinyAsync();
            var held = await _context.ExportConsignments
                .Include(c => c.Exporter)
                .Where(c => c.CustomsStatus == CustomsStatus.HoldForQuery || c.CustomsStatus == CustomsStatus.PhysicalInspectionRequired)
                .ToListAsync();

            var clearedToday = await _context.ExportConsignments
                .Include(c => c.Exporter)
                .Where(c => c.CustomsStatus == CustomsStatus.Approved && c.CustomsClearedAt >= DateTime.UtcNow.Date)
                .ToListAsync();

            var totalDuty = await _context.ExportConsignments.SumAsync(c => c.CustomsAssessedDutyINR);

            var model = new CustomsDashboardViewModel
            {
                Officer = officer,
                TotalUnderScrutiny = pendingScrutiny.Count,
                ClearedToday = clearedToday.Count,
                UnderHoldQuery = held.Count,
                TotalDutyAssessedINR = totalDuty,
                PendingScrutinyList = pendingScrutiny,
                HeldConsignments = held,
                RecentlyCleared = clearedToday.Take(6).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Scrutiny(int id)
        {
            var consignment = await _consignmentService.GetConsignmentByIdAsync(id);
            if (consignment == null) return NotFound();

            var officer = await GetActiveOfficerAsync();
            ViewBag.Officer = officer;
            ViewBag.BarcodeSvg = _qrBarcodeService.GenerateBarcode128Svg(consignment.TrackingNumber, 45);
            ViewBag.QrCodeBase64 = _qrBarcodeService.GenerateQrCodeBase64($"CUSTOMS-PBE:{consignment.PbeNumber}|IEC:{consignment.Exporter?.IecNumber}|VAL:{consignment.DeclaredValueINR}");

            // Look up HS code risks & compliance requirements
            var itemHs = consignment.Items.Select(i => i.HsCode).ToList();
            var hsInfo = await _context.HsCodeDirectories
                .Where(h => itemHs.Contains(h.HsCode))
                .ToListAsync();

            ViewBag.HsInfo = hsInfo;

            return View(consignment);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveOoc(int consignmentId, string remarks)
        {
            var officer = await GetActiveOfficerAsync();
            var consignment = await _context.ExportConsignments.FindAsync(consignmentId);
            if (consignment == null) return NotFound();

            var (success, message) = await _consignmentService.UpdateCustomsStatusAsync(
                consignment.TrackingNumber,
                CustomsStatus.Approved,
                officer.FullName,
                string.IsNullOrWhiteSpace(remarks) ? "Electronic PBE verified and cleared under Section 84 of Customs Act 1962." : remarks);

            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        public async Task<IActionResult> RaiseQuery(int consignmentId, string remarks, bool requirePhysicalInspection)
        {
            var officer = await GetActiveOfficerAsync();
            var consignment = await _context.ExportConsignments.FindAsync(consignmentId);
            if (consignment == null) return NotFound();

            var status = requirePhysicalInspection ? CustomsStatus.PhysicalInspectionRequired : CustomsStatus.HoldForQuery;
            var (success, message) = await _consignmentService.UpdateCustomsStatusAsync(
                consignment.TrackingNumber,
                status,
                officer.FullName,
                remarks);

            TempData["WarningMessage"] = message;
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        public async Task<IActionResult> AssessDuty(int consignmentId, decimal dutyAmount, string remarks)
        {
            var officer = await GetActiveOfficerAsync();
            var consignment = await _context.ExportConsignments.FindAsync(consignmentId);
            if (consignment == null) return NotFound();

            var (success, message) = await _consignmentService.UpdateCustomsStatusAsync(
                consignment.TrackingNumber,
                CustomsStatus.DutyAssessed,
                officer.FullName,
                remarks,
                dutyAmount);

            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Dashboard));
        }
    }
}
