using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AADYA_HouseOfFragrance.Models.Entities
{
    /// <summary>
    /// Represents an item in a user's shopping cart.
    /// Each cart item references a specific product variant and stores the selected quantity.
    /// </summary>
    public class ShoppingCart
    {
        /// <summary>
        /// The unique identifier for the cart item.
        /// </summary>
        [Key]
        public int CartItemId { get; set; }

        /// <summary>
        /// The ID of the user who owns this cart item.
        /// Foreign key to the User entity.
        /// </summary>
        [ForeignKey("User")]
        public int UserId { get; set; }

        /// <summary>
        /// The ID of the product variant added to the cart.
        /// Foreign key to the ProductVariant entity.
        /// </summary>
        [ForeignKey("ProductVariant")]
        public int VariantId { get; set; }

        /// <summary>
        /// The quantity of the product variant in the cart.
        /// Defaults to 1.
        /// </summary>
        public int Quantity { get; set; } = 1;

        /// <summary>
        /// The date and time when the item was added to the cart (UTC).
        /// </summary>
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // ============================================
        // NAVIGATION PROPERTIES
        // ============================================

        /// <summary>
        /// The user who owns this cart item.
        /// Represents a many-to-one relationship (many CartItems → one User).
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// The product variant associated with this cart item.
        /// Represents a many-to-one relationship (many CartItems → one ProductVariant).
        /// </summary>
        public ProductVariant? ProductVariant { get; set; }
    }
}