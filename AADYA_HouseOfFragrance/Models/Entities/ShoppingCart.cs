using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AADYA_HouseOfFragrance.Models.Entities
{
    public class ShoppingCart
    {
        [Key]
        public int CartItemId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("ProductVariant")]
        public int VariantId { get; set; }
        public int Quantity { get; set; } = 1;
        public DateTime AddedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public User User { get; set; } = new User();
        public ProductVariant ProductVariant { get; set; } = new ProductVariant();
    }
}