using System.ComponentModel.DataAnnotations;

namespace AADYA_HouseOfFragrance.Models.DTOs
{
    /// <summary>
    /// DTO for requesting a password reset OTP.
    /// </summary>
    public class ForgotPasswordRequestDto
    {
        /// <summary>
        /// The email or phone number of the user requesting the password reset.
        /// Currently, only email is supported.
        /// </summary>
        [Required(ErrorMessage = "Email or phone number is required.")]
        public string EmailOrPhone { get; set; } = string.Empty;

        /// <summary>
        /// The reset method to use. "Email" or "SMS" (currently only "Email" is implemented).
        /// </summary>
        public string? Method { get; set; }
    }

    /// <summary>
    /// DTO for verifying the OTP sent to the user.
    /// </summary>
    public class VerifyOtpDto
    {
        /// <summary>
        /// Optional token associated with the reset request.
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// The 6-digit OTP entered by the user.
        /// </summary>
        public string? Otp { get; set; }
    }

    /// <summary>
    /// DTO for resetting the password after successful OTP verification.
    /// </summary>
    public class ResetPasswordDto
    {
        /// <summary>
        /// The reset token received after OTP verification.
        /// </summary>
        [Required(ErrorMessage = "Reset token is required.")]
        public string ResetToken { get; set; } = string.Empty;

        /// <summary>
        /// The new password (must meet complexity requirements).
        /// </summary>
        [Required(ErrorMessage = "New password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Password must have at least one uppercase, one lowercase, one number, and one special character.")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// Confirmation of the new password (must match NewPassword).
        /// </summary>
        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}