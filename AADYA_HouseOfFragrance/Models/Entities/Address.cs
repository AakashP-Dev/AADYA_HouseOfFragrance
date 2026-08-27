using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AADYA_HouseOfFragrance.Models.Entities
{
    /// <summary>
    /// Represents a saved address for a user (e.g., shipping or billing address).
    /// Each address belongs to a single user and can be marked as default.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// The unique identifier for the address.
        /// </summary>
        [Key]
        public int AddressId { get; set; }

        /// <summary>
        /// The ID of the user who owns this address.
        /// Foreign key to the User entity.
        /// </summary>
        [ForeignKey("User")]
        public int UserId { get; set; }

        /// <summary>
        /// The first line of the address (e.g., street number and name).
        /// </summary>
        public string AddressLine1 { get; set; } = string.Empty;

        /// <summary>
        /// The second line of the address (e.g., apartment, suite, building) – optional.
        /// </summary>
        public string? AddressLine2 { get; set; }

        /// <summary>
        /// The city of the address.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// The state of the address.
        /// </summary>
        public string State { get; set; } = string.Empty;

        /// <summary>
        /// The postal/pincode of the address (6 digits for Indian addresses).
        /// </summary>
        public string Pincode { get; set; } = string.Empty;

        /// <summary>
        /// The phone number associated with this address.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Whether this address is the user's default shipping address.
        /// Only one address per user can be default at a time.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// A label for the address (e.g., "Home", "Office", "Other"). Defaults to "Home".
        /// </summary>
        public string Label { get; set; } = "Home";

        // ============================================
        // NAVIGATION PROPERTIES
        // ============================================

        /// <summary>
        /// The user who owns this address.
        /// </summary>
        public User? User { get; set; }
    }
}