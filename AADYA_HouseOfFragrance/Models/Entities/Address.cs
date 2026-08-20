using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AADYA_HouseOfFragrance.Models.Entities
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public string Label { get; set; } = "Home"; // Home, Office, Other

        // Navigation Property
        public User? User { get; set; }
    }
}