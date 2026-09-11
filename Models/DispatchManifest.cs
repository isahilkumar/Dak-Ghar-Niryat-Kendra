using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DakGharNiryatKendra.Models
{
    public class DispatchManifest
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string ManifestNumber { get; set; } = string.Empty; // MNF/DEL/2026/0018

        public int OriginDnkCenterId { get; set; }
        public DnkCenter? OriginDnkCenter { get; set; }

        [Required]
        [MaxLength(150)]
        public string DestinationFpo { get; set; } = "Delhi Foreign Post Office (FPO Kotla Road)";

        public DateTime DispatchDate { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string BagBarcodeNumber { get; set; } = string.Empty; // BAG-IND-DEL-98421

        [MaxLength(50)]
        public string SealNumber { get; set; } = string.Empty; // SL-DEL-48291

        public int TotalArticles { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalWeightKg { get; set; } = 0.0m;

        [MaxLength(100)]
        public string CreatedByStaff { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Status { get; set; } = "Dispatched"; // Created, Dispatched, HandedOverToFPO

        public List<ExportConsignment> Consignments { get; set; } = new List<ExportConsignment>();
    }
}
