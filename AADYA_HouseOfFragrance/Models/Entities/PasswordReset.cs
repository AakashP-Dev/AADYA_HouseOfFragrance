using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AADYA_HouseOfFragrance.Models.Entities
{
    /// <summary>
    /// Represents a password reset request with OTP (One-Time Password) for the "Forgot Password" flow.
    /// Each reset request is linked to a specific user and has an expiration time.
    /// </summary>
    /// <remarks>
    /// The Token field is reserved for future implementation of email reset links.
    /// Currently, the OTP-based flow is the primary method for password reset.
    /// </remarks>
    public class PasswordReset
    {
        /// <summary>
        /// The unique identifier for the password reset request.
        /// </summary>
        [Key]
        public int ResetId { get; set; }

        /// <summary>
        /// The ID of the user requesting the password reset.
        /// Foreign key to the User entity.
        /// </summary>
        [ForeignKey("User")]
        public int UserId { get; set; }

        /// <summary>
        /// The reset token generated after successful OTP verification.
        /// Stored in session and used to validate the final password reset step.
        /// Reserved for future implementation of email reset links as well.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// The 6-digit OTP sent to the user's email address.
        /// This is the primary method for password reset in the current implementation.
        /// </summary>
        public string Otp { get; set; } = string.Empty;

        /// <summary>
        /// The reset method used ("Email" or "SMS").
        /// Currently, only "Email" is implemented.
        /// </summary>
        public string ResetMethod { get; set; } = string.Empty;

        /// <summary>
        /// The date and time when the reset request was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The date and time when the OTP expires (UTC).
        /// Typically set to 10 minutes after creation.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Indicates whether the reset request has been used.
        /// Once an OTP is verified, this is set to true.
        /// </summary>
        public bool IsUsed { get; set; } = false;

        // ============================================
        // NAVIGATION PROPERTIES
        // ============================================

        /// <summary>
        /// The user who owns this reset request.
        /// Represents a many-to-one relationship (many PasswordResets → one User).
        /// </summary>
        public User? User { get; set; }
    }
}