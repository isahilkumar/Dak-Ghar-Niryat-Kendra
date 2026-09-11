using System.ComponentModel.DataAnnotations;

namespace DakGharNiryatKendra.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        // Password Authentication
        [MaxLength(256)]
        public string? PasswordHash { get; set; }

        public UserRole Role { get; set; } = UserRole.Exporter;

        // Exporter Specific Fields
        [MaxLength(150)]
        public string? BusinessName { get; set; }

        [MaxLength(10)]
        public string? IecNumber { get; set; } // 10-digit Import Export Code

        [MaxLength(15)]
        public string? Gstin { get; set; } // 15-digit GSTIN

        [MaxLength(20)]
        public string? AdCode { get; set; } // Authorized Dealer Code for Bank

        [MaxLength(30)]
        public string? LutNumber { get; set; } // Letter of Undertaking for zero-rated IGST

        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Pincode { get; set; }
        public bool IsVerified { get; set; } = true;

        // Postal Staff Specific Fields
        public int? AssignedDnkCenterId { get; set; }
        public DnkCenter? AssignedDnkCenter { get; set; }

        // Customs / Staff Designation
        [MaxLength(100)]
        public string? Designation { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
