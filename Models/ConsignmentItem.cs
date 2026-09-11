using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DakGharNiryatKendra.Models
{
    public class ConsignmentItem
    {
        public int Id { get; set; }

        public int ExportConsignmentId { get; set; }
        public ExportConsignment? ExportConsignment { get; set; }

        [Required]
        [MaxLength(250)]
        public string ItemDescription { get; set; } = string.Empty;

        [Required]
        [MaxLength(12)]
        public string HsCode { get; set; } = string.Empty;

        public int Quantity { get; set; } = 1;

        [MaxLength(20)]
        public string UnitOfMeasurement { get; set; } = "PCS"; // PCS, KGS, SET, NOS

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPriceINR { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPriceINR { get; set; }

        public decimal NetWeightKg { get; set; }

        [MaxLength(100)]
        public string CountryOfOrigin { get; set; } = "India";
    }
}
