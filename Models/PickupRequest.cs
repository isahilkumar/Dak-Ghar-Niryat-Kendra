using System.ComponentModel.DataAnnotations;

namespace DakGharNiryatKendra.Models
{
    public class PickupRequest
    {
        public int Id { get; set; }

        public int ExportConsignmentId { get; set; }
        public ExportConsignment? ExportConsignment { get; set; }

        public int ExporterId { get; set; }
        public User? Exporter { get; set; }

        public DateTime ScheduledDate { get; set; }
        public TimeSlot TimeSlot { get; set; } = TimeSlot.Morning_09_12;

        [Required]
        public string PickupAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Pincode { get; set; } = string.Empty;

        public PickupStatus Status { get; set; } = PickupStatus.Requested;

        [MaxLength(100)]
        public string? AssignedPostmanName { get; set; }

        [MaxLength(20)]
        public string? AssignedPostmanPhone { get; set; }

        public string? SpecialInstructions { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
