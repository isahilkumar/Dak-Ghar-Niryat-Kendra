using System.ComponentModel.DataAnnotations;

namespace DakGharNiryatKendra.Models.ViewModels
{
    public class RateCalculatorViewModel
    {
        [Required]
        public string DestinationCountryCode { get; set; } = "US";

        [Required]
        [Range(0.01, 35.0, ErrorMessage = "Weight must be between 0.01 and 35.0 Kg")]
        public decimal WeightKg { get; set; } = 1.0m;

        public decimal LengthCm { get; set; } = 25.0m;
        public decimal WidthCm { get; set; } = 20.0m;
        public decimal HeightCm { get; set; } = 10.0m;

        public decimal DeclaredValueINR { get; set; } = 5000.0m;
        public bool IncludeInsurance { get; set; } = true;

        public List<ServiceRateComparisonResult> CalculatedRates { get; set; } = new List<ServiceRateComparisonResult>();
    }

    public class ServiceRateComparisonResult
    {
        public ServiceType ServiceType { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceBadge { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePostageINR { get; set; }
        public decimal FuelSurchargeINR { get; set; }
        public decimal InsuranceINR { get; set; }
        public decimal GstINR { get; set; }
        public decimal TotalCostINR { get; set; }
        public int EstimatedDaysMin { get; set; }
        public int EstimatedDaysMax { get; set; }
        public decimal ChargeableWeightKg { get; set; }
        public bool IsEligible { get; set; } = true;
        public string? IneligibilityReason { get; set; }
    }
}
