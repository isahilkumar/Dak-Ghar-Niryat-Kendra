using System.ComponentModel.DataAnnotations;

namespace DakGharNiryatKendra.Models
{
    public class TariffZone
    {
        public int Id { get; set; }

        public ServiceType ServiceType { get; set; }

        [Required]
        [MaxLength(5)]
        public string CountryCode { get; set; } = string.Empty; // e.g. US, GB, AE, SG, DE, AU, CA, JP

        [Required]
        [MaxLength(100)]
        public string CountryName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string PostalZone { get; set; } = "Zone 1";

        public int BaseWeightGrams { get; set; } = 250;
        public decimal BaseRateINR { get; set; } = 650.00m;

        public int StepWeightGrams { get; set; } = 250;
        public decimal StepRateINR { get; set; } = 150.00m;

        public int MinDeliveryDays { get; set; } = 4;
        public int MaxDeliveryDays { get; set; } = 8;
        public decimal MaxAllowedWeightKg { get; set; } = 30.0m;

        public decimal FuelSurchargePercent { get; set; } = 10.0m;
        public decimal GstPercent { get; set; } = 18.0m;
    }
}
