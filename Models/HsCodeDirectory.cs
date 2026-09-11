using System.ComponentModel.DataAnnotations;

namespace DakGharNiryatKendra.Models
{
    public class HsCodeDirectory
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(12)]
        public string HsCode { get; set; } = string.Empty; // e.g. 6204.42, 7113.11

        [Required]
        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty; // e.g. Handicrafts, Textiles, Spices, Gems & Jewellery

        [MaxLength(10)]
        public string Chapter { get; set; } = string.Empty; // e.g. Chapter 62

        public decimal StandardGstPercent { get; set; } = 5.0m;
        public decimal ExportDutyPercent { get; set; } = 0.0m;
        public decimal RodtepRatePercent { get; set; } = 1.5m; // Remission of Duties and Taxes on Exported Products

        public bool IsRestricted { get; set; } = false;
        public string? RestrictionRemarks { get; set; }
        public string? RequiredDocuments { get; set; } // e.g. Phytosanitary Certificate, Hallmark Cert
    }
}
