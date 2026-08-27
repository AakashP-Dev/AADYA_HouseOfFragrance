namespace AADYA_HouseOfFragrance.Models.DTOs
{
    /// <summary>
    /// DTO for initiating the checkout process.
    /// </summary>
    public class CheckoutDto
    {
        /// <summary>
        /// The shipping address for the order. If null, the user's default address will be used.
        /// </summary>
        public string? ShippingAddress { get; set; }
    }

    /// <summary>
    /// Response DTO representing a complete order with all its items.
    /// </summary>
    public class OrderResponseDto
    {
        /// <summary>
        /// The unique ID of the order.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// The current status of the order (e.g., "Pending Payment", "Processing", "Dispatched", "Delivered", "Cancelled").
        /// </summary>
        public string OrderStatus { get; set; } = string.Empty;

        /// <summary>
        /// The total amount of the order.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// The date and time when the order was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The shipping address for the order.
        /// </summary>
        public string ShippingAddress { get; set; } = string.Empty;

        /// <summary>
        /// The list of items in the order.
        /// </summary>
        public List<OrderItemResponseDto> Items { get; set; } = new List<OrderItemResponseDto>();
    }

    /// <summary>
    /// Response DTO representing a single item within an order.
    /// </summary>
    public class OrderItemResponseDto
    {
        /// <summary>
        /// The unique ID of the order item.
        /// </summary>
        public int OrderItemId { get; set; }

        /// <summary>
        /// The name of the product.
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// The size of the product variant (e.g., "6ml" or "12ml").
        /// </summary>
        public string Size { get; set; } = string.Empty;

        /// <summary>
        /// The quantity of the product ordered.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The unit price at the time of sale.
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// The total price for this line item (UnitPrice × Quantity).
        /// </summary>
        public decimal Total { get; set; }
    }

    /// <summary>
    /// DTO for processing a payment on a pending order.
    /// </summary>
    public class PaymentDto
    {
        /// <summary>
        /// The ID of the order to process payment for.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// The payment method selected by the user ("Card", "UPI", or "COD").
        /// </summary>
        public string PaymentMethod { get; set; } = "Card";

        /// <summary>
        /// The card number (only required for Card payment method).
        /// </summary>
        public string? CardNumber { get; set; }

        /// <summary>
        /// The card expiry date in MM/YY format (only required for Card payment method).
        /// </summary>
        public string? CardExpiry { get; set; }

        /// <summary>
        /// The card CVV (only required for Card payment method).
        /// </summary>
        public string? CardCvv { get; set; }
    }
}