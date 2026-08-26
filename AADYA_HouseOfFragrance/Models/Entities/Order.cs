using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AADYA_HouseOfFragrance.Models.Entities
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public string OrderStatus { get; set; } = "Processing";
        public string ShippingAddress { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? PaymentMethod { get; set; }   // "Card", "UPI", "COD"
        public string? PaymentId { get; set; }      // Transaction ID from payment gateway

        // Navigation Properties
        public User? User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}