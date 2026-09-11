using System.ComponentModel.DataAnnotations;

namespace DakGharNiryatKendra.Models
{
    public class DnkCenter
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string CenterCode { get; set; } = string.Empty; // e.g. DNK-DEL-001

        [Required]
        [MaxLength(150)]
        public string CenterName { get; set; } = string.Empty; // e.g. New Delhi GPO Dak Ghar Niryat Kendra

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Pincode { get; set; } = string.Empty;

        [MaxLength(100)]
        public string ContactPerson { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public bool IsActive { get; set; } = true;
        public bool HasCustomsDesk { get; set; } = true;
        public bool HasPackagingFacility { get; set; } = true;

        [MaxLength(100)]
        public string OperatingHours { get; set; } = "09:00 AM - 05:30 PM (Mon-Sat)";

        public string? ServicesOffered { get; set; } = "EMS, Air Parcel, ITPS, Customs e-PBE Scrutiny, Packaging Support";
    }
}
