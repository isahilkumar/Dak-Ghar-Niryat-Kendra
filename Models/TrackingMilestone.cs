using System.ComponentModel.DataAnnotations;

namespace DakGharNiryatKendra.Models
{
    public class TrackingMilestone
    {
        public int Id { get; set; }

        public int ExportConsignmentId { get; set; }
        public ExportConsignment? ExportConsignment { get; set; }

        [Required]
        [MaxLength(20)]
        public string TrackingNumber { get; set; } = string.Empty;

        public DateTime EventTime { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(150)]
        public string FacilityLocation { get; set; } = string.Empty; // e.g., "Jaipur HPO DNK Counter", "Delhi FPO Kotla Road", "IGIA Hub New Delhi"

        [Required]
        [MaxLength(100)]
        public string StatusHeadline { get; set; } = string.Empty; // e.g. "Article Booked", "Customs Cleared", "Item Dispatched"

        public ConsignmentStatus StatusCategory { get; set; }

        public string? DetailedDescription { get; set; }

        [MaxLength(100)]
        public string? PerformedBy { get; set; } // Officer / Agent Name

        public string? NextExpectedAction { get; set; }
    }
}
