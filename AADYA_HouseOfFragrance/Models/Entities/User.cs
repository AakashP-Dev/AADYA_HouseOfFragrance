using System.ComponentModel.DataAnnotations;

namespace AADYA_HouseOfFragrance.Models.Entities
{
    /// <summary>
    /// Represents a registered user of the application.
    /// Tracks authentication, profile information, roles, and verification status.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The unique identifier for the user.
        /// </summary>
        [Key]
        public int UserId { get; set; }

        /// <summary>
        /// The full name of the user.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// The email address of the user (must be unique).
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The phone number of the user (10-digit Indian mobile number).
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// The hashed password of the user (BCrypt hash).
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// The default delivery address of the user.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// The Base64-encoded avatar image (optional).
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// Indicates whether the user has administrative privileges.
        /// </summary>
        public bool IsAdmin { get; set; } = false;

        /// <summary>
        /// The date and time when the user account was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Indicates whether the user is a Super Admin.
        /// Super Admins can promote/demote other users to/from admin status.
        /// </summary>
        public bool IsSuperAdmin { get; set; } = false;

        /// <summary>
        /// Indicates whether the user's phone number has been verified.
        /// Currently not implemented; reserved for future use.
        /// </summary>
        public bool IsPhoneVerified { get; set; } = false;

        /// <summary>
        /// Indicates whether the user's email address has been verified.
        /// Currently not implemented; reserved for future use.
        /// </summary>
        public bool IsEmailVerified { get; set; } = false;

        // ============================================
        // NAVIGATION PROPERTIES
        // ============================================

        /// <summary>
        /// The collection of orders placed by the user.
        /// Represents a one-to-many relationship (one User → many Orders).
        /// </summary>
        public ICollection<Order> Orders { get; set; } = new List<Order>();

        /// <summary>
        /// The collection of shopping cart items belonging to the user.
        /// Represents a one-to-many relationship (one User → many ShoppingCart items).
        /// </summary>
        public ICollection<ShoppingCart> ShoppingCartItems { get; set; } = new List<ShoppingCart>();

        /// <summary>
        /// The collection of saved addresses for the user.
        /// Represents a one-to-many relationship (one User → many Addresses).
        /// </summary>
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}