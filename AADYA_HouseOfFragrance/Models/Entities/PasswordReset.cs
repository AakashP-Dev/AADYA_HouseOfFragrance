using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AADYA_HouseOfFragrance.Models.Entities
{
    public class PasswordReset
    {
        [Key]
        public int ResetId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public string Token { get; set; } = string.Empty;  // For email links
        public string Otp { get; set; } = string.Empty;    // For SMS OTP
        public string ResetMethod { get; set; } = string.Empty; // "Email" or "SMS"
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;

        public User? User { get; set; }
    }
}