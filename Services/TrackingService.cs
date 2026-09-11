using DakGharNiryatKendra.Data;
using DakGharNiryatKendra.Models;
using DakGharNiryatKendra.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DakGharNiryatKendra.Services
{
    public class TrackingService : ITrackingService
    {
        private readonly ApplicationDbContext _context;

        public TrackingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TrackingViewModel> TrackAsync(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
            {
                return new TrackingViewModel
                {
                    TrackingNumber = string.Empty,
                    IsFound = false,
                    ErrorMessage = "Please enter an Article / Tracking number (e.g. EM892104928IN or LP928410293IN)."
                };
            }

            var cleanNum = trackingNumber.Trim().ToUpperInvariant();
            var consignment = await _context.ExportConsignments
                .Include(c => c.Exporter)
                .Include(c => c.DnkCenter)
                .Include(c => c.Items)
                .Include(c => c.Milestones.OrderByDescending(m => m.EventTime))
                .FirstOrDefaultAsync(c => c.TrackingNumber == cleanNum || c.PbeNumber == cleanNum);

            if (consignment == null)
            {
                return new TrackingViewModel
                {
                    TrackingNumber = cleanNum,
                    IsFound = false,
                    ErrorMessage = $"No tracking record found for '{cleanNum}'. Please check the Article number or verify with your nearest Dak Ghar Niryat Kendra."
                };
            }

            return new TrackingViewModel
            {
                TrackingNumber = consignment.TrackingNumber,
                Consignment = consignment,
                IsFound = true,
                Milestones = consignment.Milestones.OrderByDescending(m => m.EventTime).ToList()
            };
        }

        public async Task<bool> AddMilestoneAsync(string trackingNumber, string facilityLocation, string headline, ConsignmentStatus status, string description, string? performedBy = null)
        {
            var cleanNum = trackingNumber.Trim().ToUpperInvariant();
            var consignment = await _context.ExportConsignments
                .FirstOrDefaultAsync(c => c.TrackingNumber == cleanNum);

            if (consignment == null) return false;

            consignment.Status = status;
            var milestone = new TrackingMilestone
            {
                ExportConsignmentId = consignment.Id,
                TrackingNumber = consignment.TrackingNumber,
                EventTime = DateTime.UtcNow,
                FacilityLocation = facilityLocation,
                StatusHeadline = headline,
                StatusCategory = status,
                DetailedDescription = description,
                PerformedBy = performedBy
            };

            _context.TrackingMilestones.Add(milestone);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string Message, ConsignmentStatus? NewStatus)> SimulateNextStepAsync(string trackingNumber)
        {
            var cleanNum = trackingNumber.Trim().ToUpperInvariant();
            var consignment = await _context.ExportConsignments
                .Include(c => c.DnkCenter)
                .FirstOrDefaultAsync(c => c.TrackingNumber == cleanNum);

            if (consignment == null)
            {
                return (false, "Article not found", null);
            }

            ConsignmentStatus nextStatus;
            string location;
            string headline;
            string details;

            switch (consignment.Status)
            {
                case ConsignmentStatus.Booked:
                case ConsignmentStatus.PickupScheduled:
                    nextStatus = ConsignmentStatus.InductedAtDNK;
                    consignment.InductedAt = DateTime.UtcNow;
                    consignment.VerifiedWeightKg = consignment.ActualWeightKg;
                    location = consignment.DnkCenter?.CenterName ?? "New Delhi GPO DNK";
                    headline = "Article Inducted at Dak Ghar Niryat Kendra";
                    details = "Package physically accepted, barcoded and weighed. Prepared for dispatch to Office of Exchange.";
                    break;

                case ConsignmentStatus.InductedAtDNK:
                    nextStatus = ConsignmentStatus.BaggedAndManifested;
                    location = consignment.DnkCenter?.CenterName ?? "Jaipur HPO DNK";
                    headline = "Bagged and Dispatched to Foreign Post Office (FPO)";
                    details = "Consignment sorted into international air mail container bag and dispatched via secure postal transit.";
                    break;

                case ConsignmentStatus.BaggedAndManifested:
                case ConsignmentStatus.DispatchedToFPO:
                    nextStatus = ConsignmentStatus.ReceivedAtFPO;
                    location = "Delhi Foreign Post Office (FPO Kotla Road)";
                    headline = "Received at Office of Exchange / FPO";
                    details = "Article received at Foreign Post Office. Handed over to Customs Postal Appraising Wing for export scrutiny.";
                    break;

                case ConsignmentStatus.ReceivedAtFPO:
                case ConsignmentStatus.CustomsHold:
                    nextStatus = ConsignmentStatus.CustomsCleared;
                    consignment.CustomsStatus = CustomsStatus.Approved;
                    consignment.CustomsClearedAt = DateTime.UtcNow;
                    location = "Delhi FPO Customs Appraising Wing";
                    headline = "Customs Out of Charge (OOC) Granted";
                    details = "Electronic Postal Bill of Export (PBE) scrutinized and cleared. Zero duty applicable.";
                    break;

                case ConsignmentStatus.CustomsCleared:
                    nextStatus = ConsignmentStatus.DepartedOriginCountry;
                    location = "Indira Gandhi International Airport Cargo Hub (DEL)";
                    headline = "Departed Origin Country (India)";
                    details = $"Dispatched on International Scheduled Air Flight to destination gateway ({consignment.RecipientCountry}).";
                    break;

                case ConsignmentStatus.DepartedOriginCountry:
                    nextStatus = ConsignmentStatus.ArrivedDestinationHub;
                    location = $"{consignment.RecipientCity} International Postal Exchange Hub";
                    headline = $"Arrived in Destination Country ({consignment.RecipientCountry})";
                    details = $"Package safely received by destination postal administration in {consignment.RecipientCountry}.";
                    break;

                case ConsignmentStatus.ArrivedDestinationHub:
                    nextStatus = ConsignmentStatus.OutForDelivery;
                    location = $"{consignment.RecipientCity} Central Delivery Depot ({consignment.RecipientPostalCode})";
                    headline = "Out for Delivery";
                    details = "Package assigned to local postal courier for doorstep delivery to recipient.";
                    break;

                case ConsignmentStatus.OutForDelivery:
                    nextStatus = ConsignmentStatus.Delivered;
                    consignment.DeliveredAt = DateTime.UtcNow;
                    location = $"{consignment.RecipientAddress}, {consignment.RecipientCity}";
                    headline = "Item Successfully Delivered";
                    details = $"Delivered to consignee {consignment.RecipientName}. Proof of Delivery (e-POD) recorded.";
                    break;

                default:
                    return (true, "Article is already in final Delivered state.", consignment.Status);
            }

            consignment.Status = nextStatus;
            var milestone = new TrackingMilestone
            {
                ExportConsignmentId = consignment.Id,
                TrackingNumber = consignment.TrackingNumber,
                EventTime = DateTime.UtcNow,
                FacilityLocation = location,
                StatusHeadline = headline,
                StatusCategory = nextStatus,
                DetailedDescription = details,
                PerformedBy = "India Post Automated Tracking Hub"
            };

            _context.TrackingMilestones.Add(milestone);
            await _context.SaveChangesAsync();

            return (true, $"Simulated progression: {headline}", nextStatus);
        }
    }
}
