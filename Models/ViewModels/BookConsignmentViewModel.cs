using System.ComponentModel.DataAnnotations;

namespace DakGharNiryatKendra.Models.ViewModels
{
    public class BookConsignmentViewModel
    {
        // Service Selection
        [Required(ErrorMessage = "Please select a postal export service.")]
        public ServiceType ServiceType { get; set; } = ServiceType.EMS_SpeedPost;

        [Required(ErrorMessage = "Please select a PBE Type.")]
        public PbeType PbeType { get; set; } = PbeType.PBE_I;

        [Required(ErrorMessage = "Please select an induction Dak Ghar Niryat Kendra.")]
        public int DnkCenterId { get; set; }

        // Consignee / Recipient
        [Required(ErrorMessage = "Recipient full name is required.")]
        [MaxLength(150)]
        public string RecipientName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? RecipientCompany { get; set; }

        [Required(ErrorMessage = "Recipient email is required.")]
        [EmailAddress]
        public string RecipientEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Recipient phone is required.")]
        public string RecipientPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Recipient delivery address is required.")]
        public string RecipientAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Recipient city is required.")]
        public string RecipientCity { get; set; } = string.Empty;

        public string? RecipientState { get; set; }

        [Required(ErrorMessage = "Recipient postal / ZIP code is required.")]
        public string RecipientPostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Destination country is required.")]
        public string DestinationCountryCode { get; set; } = "US";

        public string RecipientCountry { get; set; } = "United States";

        // Dimensions & Weight
        [Range(0.01, 35.0, ErrorMessage = "Actual weight must be between 0.01 and 35.0 Kg.")]
        public decimal ActualWeightKg { get; set; } = 0.5m;

        [Range(1.0, 300.0, ErrorMessage = "Length must be at least 1 cm.")]
        public decimal LengthCm { get; set; } = 20.0m;

        [Range(1.0, 300.0, ErrorMessage = "Width must be at least 1 cm.")]
        public decimal WidthCm { get; set; } = 15.0m;

        [Range(1.0, 300.0, ErrorMessage = "Height must be at least 1 cm.")]
        public decimal HeightCm { get; set; } = 10.0m;

        // Customs & Invoice
        public string NatureOfExport { get; set; } = "Sale of Goods (Commercial)";
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; } = DateTime.Today;
        public string? LutNumber { get; set; }

        public string CurrencyCode { get; set; } = "USD";
        public decimal DeclaredValueForeign { get; set; } = 100.0m;
        public decimal ExchangeRateToINR { get; set; } = 86.50m;
        public decimal DeclaredValueINR { get; set; } = 8650.0m;

        // Doorstep Pickup
        public bool RequestPickup { get; set; } = false;
        public DateTime? ScheduledPickupDate { get; set; } = DateTime.Today.AddDays(1);
        public TimeSlot PickupTimeSlot { get; set; } = TimeSlot.Morning_09_12;
        public string? PickupSpecialInstructions { get; set; }

        // Dynamic Items JSON or Collection
        public List<ConsignmentItemInputModel> Items { get; set; } = new List<ConsignmentItemInputModel>();
    }

    public class ConsignmentItemInputModel
    {
        [Required]
        public string ItemDescription { get; set; } = string.Empty;

        [Required]
        public string HsCode { get; set; } = string.Empty;

        public int Quantity { get; set; } = 1;
        public string UnitOfMeasurement { get; set; } = "PCS";
        public decimal UnitPriceINR { get; set; } = 0.0m;
        public decimal TotalPriceINR { get; set; } = 0.0m;
        public decimal NetWeightKg { get; set; } = 0.5m;
        public string CountryOfOrigin { get; set; } = "India";
    }
}
