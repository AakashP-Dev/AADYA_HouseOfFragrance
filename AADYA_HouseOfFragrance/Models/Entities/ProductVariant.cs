using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AADYA_HouseOfFragrance.Models.Entities
{
    public class ProductVariant
    {
        [Key]
        public int VariantId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public string Size { get; set; } = string.Empty;  // "6ml" or "12ml"
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        // Navigation Properties
        public Product Product { get; set; } = new Product();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<ShoppingCart> ShoppingCartItems { get; set; } = new List<ShoppingCart>();
    }
}