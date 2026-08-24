using System.ComponentModel.DataAnnotations;

namespace AADYA_HouseOfFragrance.Models.DTOs
{
    public class ForgotPasswordRequestDto
    {
        [Required(ErrorMessage = "Email or phone number is required.")]
        public string EmailOrPhone { get; set; } = string.Empty;

        public string? Method { get; set; } // "Email" or "SMS"
    }

    public class VerifyOtpDto
    {
        public string? Token { get; set; }
        public string? Otp { get; set; }
    }

    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Reset token is required.")]
        public string ResetToken { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Password must have at least one uppercase, one lowercase, one number, and one special character.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}