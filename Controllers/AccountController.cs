using Microsoft.AspNetCore.Mvc;
using DakGharNiryatKendra.Data;
using DakGharNiryatKendra.Models;
using DakGharNiryatKendra.Models.ViewModels;
using DakGharNiryatKendra.Services;
using Microsoft.EntityFrameworkCore;

namespace DakGharNiryatKendra.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public AccountController(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> SwitchUser(string username, string? returnUrl)
        {
            await _authService.SetCurrentUserAsync(username);
            var user = await _authService.GetCurrentUserAsync();

            TempData["SuccessMessage"] = $"Switched persona to: {user?.FullName} ({user?.Role})";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return user?.Role switch
            {
                UserRole.Exporter => RedirectToAction("Dashboard", "Exporter"),
                UserRole.PostalStaff => RedirectToAction("Dashboard", "PostalStaff"),
                UserRole.CustomsOfficer => RedirectToAction("Dashboard", "Customs"),
                UserRole.Admin => RedirectToAction("Dashboard", "Admin"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var user = await _authService.AuthenticateWithPasswordAsync(model.UsernameOrEmail, model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username/email or password. Please try again.");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            TempData["SuccessMessage"] = $"Welcome back, {user.FullName}!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return user.Role switch
            {
                UserRole.Exporter => RedirectToAction("Dashboard", "Exporter"),
                UserRole.PostalStaff => RedirectToAction("Dashboard", "PostalStaff"),
                UserRole.CustomsOfficer => RedirectToAction("Dashboard", "Customs"),
                UserRole.Admin => RedirectToAction("Dashboard", "Admin"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, error, user) = await _authService.RegisterUserAsync(model);

            if (!success)
            {
                ModelState.AddModelError("", error);
                return View(model);
            }

            TempData["SuccessMessage"] = $"Welcome to Dak Ghar Niryat Kendra, {user!.FullName}! Your MSME Exporter account has been created successfully.";
            return RedirectToAction("Dashboard", "Exporter");
        }

        // AJAX endpoint for checking username availability
        [HttpGet]
        public async Task<IActionResult> CheckUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            {
                return Json(new { available = false, message = "Username must be at least 3 characters" });
            }

            var taken = await _authService.IsUsernameTakenAsync(username);
            return Json(new
            {
                available = !taken,
                message = taken ? "This username is already taken" : "Username is available"
            });
        }

        // AJAX endpoint for checking email availability
        [HttpGet]
        public async Task<IActionResult> CheckEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Json(new { available = false, message = "Email is required" });
            }

            var taken = await _authService.IsEmailTakenAsync(email);
            return Json(new
            {
                available = !taken,
                message = taken ? "An account with this email already exists" : "Email is available"
            });
        }

        [HttpGet("/User/Welcome")]
        [HttpGet("/Account/Welcome")]
        [HttpGet("/Exporter/Welcome")]
        [HttpGet("/PostalStaff/Welcome")]
        [HttpGet("/Customs/Welcome")]
        [HttpGet("/Admin/Welcome")]
        public async Task<IActionResult> Welcome()
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                return RedirectToAction("Login");
            }

            var assignedKendra = await _context.DnkCenters
                .FirstOrDefaultAsync(c => c.Id == currentUser.AssignedDnkCenterId) 
                ?? await _context.DnkCenters.FirstOrDefaultAsync();

            var userConsignments = await _context.ExportConsignments
                .Include(c => c.Items)
                .Include(c => c.Milestones)
                .Include(c => c.DnkCenter)
                .Where(c => c.ExporterId == currentUser.Id)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            // If user has no shipments yet, fetch recent system shipments for demo showcase
            if (!userConsignments.Any())
            {
                userConsignments = await _context.ExportConsignments
                    .Include(c => c.Items)
                    .Include(c => c.Milestones)
                    .Include(c => c.DnkCenter)
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(5)
                    .ToListAsync();
            }

            var totalDeclaredVal = userConsignments.Sum(c => c.DeclaredValueINR);
            var totalDispatched = userConsignments.Count;
            var totalDelivered = userConsignments.Count(c => c.Status == ConsignmentStatus.Delivered);
            var inTransit = userConsignments.Count(c => c.Status != ConsignmentStatus.Delivered && c.Status != ConsignmentStatus.Draft);

            // Postal Staff metrics
            var pendingInductions = await _context.ExportConsignments.CountAsync(c => c.Status == ConsignmentStatus.Booked || c.Status == ConsignmentStatus.PickupScheduled);
            var totalManifests = await _context.DispatchManifests.CountAsync();
            var pendingPickups = await _context.PickupRequests.CountAsync(p => p.Status == PickupStatus.Requested || p.Status == PickupStatus.Assigned);

            // Customs metrics
            var pendingCustomsScrutiny = await _context.ExportConsignments.CountAsync(c => c.CustomsStatus == CustomsStatus.PendingVerification);
            var clearedToday = await _context.ExportConsignments.CountAsync(c => c.CustomsStatus == CustomsStatus.Approved);
            var holdForQuery = await _context.ExportConsignments.CountAsync(c => c.CustomsStatus == CustomsStatus.HoldForQuery);

            // Admin metrics
            var totalCenters = await _context.DnkCenters.CountAsync();
            var totalExporters = await _context.Users.CountAsync(u => u.Role == UserRole.Exporter);
            var nationalTurnover = (await _context.ExportConsignments.Select(c => (decimal?)c.DeclaredValueINR).SumAsync()) ?? 0m;

            ViewBag.CurrentUser = currentUser;
            ViewBag.AssignedKendra = assignedKendra;
            ViewBag.UserConsignments = userConsignments;
            ViewBag.TotalDeclaredVal = totalDeclaredVal;
            ViewBag.TotalDispatched = totalDispatched;
            ViewBag.TotalDelivered = totalDelivered;
            ViewBag.InTransit = inTransit;

            ViewBag.PendingInductions = pendingInductions;
            ViewBag.TotalManifests = totalManifests;
            ViewBag.PendingPickups = pendingPickups;

            ViewBag.PendingCustomsScrutiny = pendingCustomsScrutiny;
            ViewBag.ClearedToday = clearedToday;
            ViewBag.HoldForQuery = holdForQuery;

            ViewBag.TotalCenters = totalCenters;
            ViewBag.TotalExporters = totalExporters;
            ViewBag.NationalTurnover = nationalTurnover;

            ViewBag.PopularHsCodes = await _context.HsCodeDirectories.Take(6).ToListAsync();

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }
    }
}
