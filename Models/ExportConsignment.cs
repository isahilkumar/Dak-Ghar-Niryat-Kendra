using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DakGharNiryatKendra.Models
{
    public class ExportConsignment
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string TrackingNumber { get; set; } = string.Empty; // S10 standard, e.g. EM829471920IN

        [Required]
        [MaxLength(35)]
        public string PbeNumber { get; set; } = string.Empty; // e.g. PBE/2026/DEL/00492

        public PbeType PbeType { get; set; } = PbeType.PBE_I;
        public ServiceType ServiceType { get; set; } = ServiceType.EMS_SpeedPost;
        public ConsignmentStatus Status { get; set; } = ConsignmentStatus.Booked;
        public CustomsStatus CustomsStatus { get; set; } = CustomsStatus.PendingVerification;

        // Exporter & DNK Center
        public int ExporterId { get; set; }
        public User? Exporter { get; set; }

        public int DnkCenterId { get; set; }
        public DnkCenter? DnkCenter { get; set; }

        // Consignee (Recipient) Details
        [Required]
        [MaxLength(150)]
        public string RecipientName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? RecipientCompany { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string RecipientEmail { get; set; } = string.Empty;

        [Required]
        [MaxLength(25)]
        public string RecipientPhone { get; set; } = string.Empty;

        [Required]
        public string RecipientAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string RecipientCity { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? RecipientState { get; set; }

        [Required]
        [MaxLength(20)]
        public string RecipientPostalCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string RecipientCountry { get; set; } = string.Empty;

        [Required]
        [MaxLength(5)]
        public string DestinationCountryCode { get; set; } = string.Empty; // US, GB, AE, etc.

        // Package Dimensions & Weighment
        public decimal ActualWeightKg { get; set; } = 0.5m;
        public decimal LengthCm { get; set; } = 20.0m;
        public decimal WidthCm { get; set; } = 15.0m;
        public decimal HeightCm { get; set; } = 10.0m;
        public decimal VolumetricWeightKg { get; set; } = 0.6m; // (L*W*H)/5000
        public decimal ChargeableWeightKg { get; set; } = 0.6m;

        // Commercial / Customs Information
        [MaxLength(50)]
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }

        [MaxLength(30)]
        public string? LutNumber { get; set; }

        [MaxLength(50)]
        public string NatureOfExport { get; set; } = "Sale of Goods (Commercial)"; // Sale of Goods, Gift, Commercial Sample, Return

        [MaxLength(10)]
        public string CurrencyCode { get; set; } = "INR"; // USD, EUR, GBP, INR

        [Column(TypeName = "decimal(18,2)")]
        public decimal DeclaredValueForeign { get; set; } = 0.0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ExchangeRateToINR { get; set; } = 1.0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DeclaredValueINR { get; set; } = 0.0m;

        // Pricing Breakdown
        [Column(TypeName = "decimal(18,2)")]
        public decimal PostageAmountINR { get; set; } = 0.0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal FuelSurchargeINR { get; set; } = 0.0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal InsuranceAmountINR { get; set; } = 0.0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal GstAmountINR { get; set; } = 0.0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalShippingCostINR { get; set; } = 0.0m;

        // Customs Scrutiny & Assessment
        [Column(TypeName = "decimal(18,2)")]
        public decimal CustomsAssessedDutyINR { get; set; } = 0.0m;

        public string? CustomsOfficerRemarks { get; set; }
        public string? CustomsInspectionLog { get; set; }
        public string? ScrutinyOfficerName { get; set; }
        public DateTime? CustomsClearedAt { get; set; }

        // Induction Details
        public DateTime? InductedAt { get; set; }
        public string? InductedByStaffName { get; set; }
        public decimal? VerifiedWeightKg { get; set; }

        // Manifest & Dispatch
        public int? DispatchManifestId { get; set; }
        public DispatchManifest? DispatchManifest { get; set; }

        // Pickup Details
        public bool IsPickupRequested { get; set; } = false;
        public PickupStatus PickupStatus { get; set; } = PickupStatus.NotRequested;
        public DateTime? ScheduledPickupDate { get; set; }
        public TimeSlot? PickupTimeSlot { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeliveredAt { get; set; }

        // Relations
        public List<ConsignmentItem> Items { get; set; } = new List<ConsignmentItem>();
        public List<TrackingMilestone> Milestones { get; set; } = new List<TrackingMilestone>();
    }
}
