using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AADYA_HouseOfFragrance.Models.Entities
{
    /// <summary>
    /// Represents a specific size/price variant of a product.
    /// Each product can have multiple variants (e.g., 6ml and 12ml bottles).
    /// </summary>
    public class ProductVariant
    {
        /// <summary>
        /// The unique identifier for the variant.
        /// </summary>
        [Key]
        public int VariantId { get; set; }

        /// <summary>
        /// The ID of the product this variant belongs to.
        /// Foreign key to the Product entity.
        /// </summary>
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        /// <summary>
        /// The size of the variant (e.g., "6ml" or "12ml").
        /// </summary>
        public string Size { get; set; } = string.Empty;

        /// <summary>
        /// The price of this variant.
        /// Stored as decimal with 18,2 precision for accurate monetary values.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        /// <summary>
        /// The current stock quantity available for this variant.
        /// </summary>
        public int StockQuantity { get; set; }

        // ============================================
        // NAVIGATION PROPERTIES
        // ============================================

        /// <summary>
        /// The product that this variant belongs to.
        /// Represents a many-to-one relationship (many Variants → one Product).
        /// </summary>
        public Product? Product { get; set; }

        /// <summary>
        /// The collection of order items referencing this variant.
        /// Represents a one-to-many relationship (one Variant → many OrderItems).
        /// </summary>
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        /// <summary>
        /// The collection of shopping cart items referencing this variant.
        /// Represents a one-to-many relationship (one Variant → many ShoppingCart items).
        /// </summary>
        public ICollection<ShoppingCart> ShoppingCartItems { get; set; } = new List<ShoppingCart>();
    }
}