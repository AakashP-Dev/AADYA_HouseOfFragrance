using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AADYA_HouseOfFragrance.Models.Entities
{
    /// <summary>
    /// Represents a customer order containing one or more order items.
    /// Tracks order status, shipping address, total amount, and payment information.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// The unique identifier for the order.
        /// </summary>
        [Key]
        public int OrderId { get; set; }

        /// <summary>
        /// The ID of the user who placed the order.
        /// Foreign key to the User entity.
        /// </summary>
        [ForeignKey("User")]
        public int UserId { get; set; }

        /// <summary>
        /// The current status of the order.
        /// Possible values: "Pending Payment", "Processing", "Dispatched", "Delivered", "Cancelled".
        /// Defaults to "Processing".
        /// </summary>
        public string OrderStatus { get; set; } = "Processing";

        /// <summary>
        /// The address where the order should be delivered.
        /// </summary>
        public string ShippingAddress { get; set; } = string.Empty;

        /// <summary>
        /// The total amount of the order (sum of all line items).
        /// Stored as decimal with 18,2 precision for accurate monetary values.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// The date and time when the order was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The payment method used for the order.
        /// Possible values: "Card", "UPI", "COD" (Cash on Delivery).
        /// Nullable for orders that haven't completed payment yet.
        /// </summary>
        public string? PaymentMethod { get; set; }

        /// <summary>
        /// The transaction ID from the payment gateway.
        /// For dummy payments: "PAY-DUMMY-{ticks}" or "COD-{ticks}".
        /// Nullable for orders that haven't completed payment yet.
        /// </summary>
        public string? PaymentId { get; set; }

        // ============================================
        // NAVIGATION PROPERTIES
        // ============================================

        /// <summary>
        /// The user who placed the order.
        /// Represents a many-to-one relationship (many Orders → one User).
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// The collection of items (line items) included in this order.
        /// Represents a one-to-many relationship (one Order → many OrderItems).
        /// </summary>
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}