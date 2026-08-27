using System.ComponentModel.DataAnnotations;

namespace AADYA_HouseOfFragrance.Models.DTOs
{
    /// <summary>
    /// DTO for user registration. Contains all required fields with validation.
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Full name of the user (letters and spaces only).
        /// </summary>
        [Required(ErrorMessage = "Full name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Email address (must be a valid email format).
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Phone number (10-digit Indian mobile number starting with 6, 7, 8, or 9).
        /// </summary>
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit Indian mobile number starting with 6, 7, 8, or 9.")]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Password (minimum 8 characters, with at least one uppercase, one lowercase, one digit, and one special character).
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Password must have at least one uppercase, one lowercase, one number, and one special character.")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Default delivery address.
        /// </summary>
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for user login.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Email address (required).
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password (required).
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Whether to persist the login session across browser restarts (30 days).
        /// </summary>
        public bool RememberMe { get; set; } = false;

        /// <summary>
        /// Whether the user is attempting to log in as an admin.
        /// </summary>
        public bool AdminLogin { get; set; } = false;
    }

    /// <summary>
    /// Response DTO returned after successful authentication.
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// JWT token for subsequent API calls.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// User's full name.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// User's email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's unique identifier.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Indicates whether the user has admin privileges.
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Indicates whether the user is a super admin (can promote/demote admins).
        /// </summary>
        public bool IsSuperAdmin { get; set; }

        /// <summary>
        /// Optional message (e.g., success or error description).
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}