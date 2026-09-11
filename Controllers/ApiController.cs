using Microsoft.AspNetCore.Mvc;
using DakGharNiryatKendra.Data;
using DakGharNiryatKendra.Models;
using DakGharNiryatKendra.Services;
using Microsoft.EntityFrameworkCore;

namespace DakGharNiryatKendra.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITariffCalculatorService _tariffService;
        private readonly ITrackingService _trackingService;

        public ApiController(
            ApplicationDbContext context,
            ITariffCalculatorService tariffService,
            ITrackingService trackingService)
        {
            _context = context;
            _tariffService = tariffService;
            _trackingService = trackingService;
        }

        [HttpGet("rates")]
        public async Task<IActionResult> CalculateRates(
            [FromQuery] string country = "US",
            [FromQuery] decimal weight = 1.0m,
            [FromQuery] decimal length = 20.0m,
            [FromQuery] decimal width = 15.0m,
            [FromQuery] decimal height = 10.0m,
            [FromQuery] decimal value = 5000.0m,
            [FromQuery] bool insurance = true)
        {
            var rates = await _tariffService.CalculateRatesAsync(country, weight, length, width, height, value, insurance);
            decimal volumetricWeight = _tariffService.CalculateVolumetricWeight(length, width, height);

            return Ok(new
            {
                country,
                actualWeightKg = weight,
                volumetricWeightKg = volumetricWeight,
                chargeableWeightKg = Math.Max(weight, volumetricWeight),
                services = rates
            });
        }

        [HttpGet("hscodes")]
        public async Task<IActionResult> SearchHsCodes([FromQuery] string query = "")
        {
            var q = query.Trim().ToLowerInvariant();
            var results = await _context.HsCodeDirectories
                .Where(h => h.HsCode.Contains(q) || h.Description.ToLower().Contains(q) || h.Category.ToLower().Contains(q))
                .Take(15)
                .ToListAsync();

            return Ok(results);
        }

        [HttpGet("kendras")]
        public async Task<IActionResult> GetKendras([FromQuery] string? pincode, [FromQuery] string? state)
        {
            var query = _context.DnkCenters.AsQueryable();

            if (!string.IsNullOrEmpty(pincode))
            {
                query = query.Where(c => c.Pincode.StartsWith(pincode.Trim()));
            }

            if (!string.IsNullOrEmpty(state))
            {
                query = query.Where(c => c.State.ToLower() == state.Trim().ToLower());
            }

            var kendras = await query.ToListAsync();
            return Ok(kendras);
        }

        [HttpGet("track/{trackingNumber}")]
        public async Task<IActionResult> GetTracking(string trackingNumber)
        {
            var result = await _trackingService.TrackAsync(trackingNumber);
            if (!result.IsFound)
            {
                return NotFound(new { message = result.ErrorMessage });
            }

            return Ok(new
            {
                trackingNumber = result.Consignment?.TrackingNumber,
                pbeNumber = result.Consignment?.PbeNumber,
                status = result.Consignment?.Status.ToString(),
                serviceType = result.Consignment?.ServiceType.ToString(),
                recipient = new
                {
                    name = result.Consignment?.RecipientName,
                    country = result.Consignment?.RecipientCountry,
                    city = result.Consignment?.RecipientCity
                },
                milestones = result.Milestones.Select(m => new
                {
                    time = m.EventTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    facility = m.FacilityLocation,
                    headline = m.StatusHeadline,
                    category = m.StatusCategory.ToString(),
                    details = m.DetailedDescription
                })
            });
        }

        [HttpPost("simulate-next/{trackingNumber}")]
        public async Task<IActionResult> SimulateNextStep(string trackingNumber)
        {
            var (success, message, newStatus) = await _trackingService.SimulateNextStepAsync(trackingNumber);
            return Ok(new { success, message, newStatus = newStatus?.ToString() });
        }
    }
}
