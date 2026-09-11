using Microsoft.AspNetCore.Mvc;
using DakGharNiryatKendra.Services;

namespace DakGharNiryatKendra.Controllers
{
    public class TrackingController : Controller
    {
        private readonly ITrackingService _trackingService;

        public TrackingController(ITrackingService trackingService)
        {
            _trackingService = trackingService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return View(new Models.ViewModels.TrackingViewModel());
            }

            var model = await _trackingService.TrackAsync(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Track(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index), new { id = trackingNumber.Trim().ToUpperInvariant() });
        }

        [HttpPost]
        public async Task<IActionResult> SimulateStep(string trackingNumber)
        {
            var (success, message, newStatus) = await _trackingService.SimulateNextStepAsync(trackingNumber);
            TempData["SimulatorMessage"] = message;
            TempData["SimulatorSuccess"] = success;
            return RedirectToAction(nameof(Index), new { id = trackingNumber });
        }
    }
}
