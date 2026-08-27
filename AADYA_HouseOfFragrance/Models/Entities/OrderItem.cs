using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AADYA_HouseOfFragrance.Models.Entities
{
    /// <summary>
    /// Represents a single item within an order.
    /// Each order item references a product variant and stores the quantity and price at the time of sale.
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// The unique identifier for the order item.
        /// </summary>
        [Key]
        public int OrderItemId { get; set; }

        /// <summary>
        /// The ID of the order this item belongs to.
        /// Foreign key to the Order entity.
        /// </summary>
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        /// <summary>
        /// The ID of the product variant ordered.
        /// Foreign key to the ProductVariant entity.
        /// </summary>
        [ForeignKey("ProductVariant")]
        public int VariantId { get; set; }

        /// <summary>
        /// The quantity of the product ordered.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The unit price of the product at the time of sale.
        /// Stored as decimal with 18,2 precision for accurate monetary values.
        /// This value is immutable and does not change even if the product price is updated later.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPriceAtSale { get; set; }

        // ============================================
        // NAVIGATION PROPERTIES
        // ============================================

        /// <summary>
        /// The order that contains this item.
        /// Represents a many-to-one relationship (many OrderItems → one Order).
        /// </summary>
        public Order? Order { get; set; }

        /// <summary>
        /// The product variant that was ordered.
        /// Represents a many-to-one relationship (many OrderItems → one ProductVariant).
        /// </summary>
        public ProductVariant? ProductVariant { get; set; }
    }
}