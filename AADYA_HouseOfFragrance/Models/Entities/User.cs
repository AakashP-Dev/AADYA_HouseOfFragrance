using System.ComponentModel.DataAnnotations;

namespace AADYA_HouseOfFragrance.Models.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public bool IsAdmin { get; set; } = false;  // ← NEW
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsSuperAdmin { get; set; } = false;

        // Navigation Properties
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<ShoppingCart> ShoppingCartItems { get; set; } = new List<ShoppingCart>();
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}