namespace AADYA_HouseOfFragrance.Models.DTOs
{
    public class CheckoutDto
    {
        public string? ShippingAddress { get; set; }
    }

    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public List<OrderItemResponseDto> Items { get; set; } = new List<OrderItemResponseDto>();
    }

    public class OrderItemResponseDto
    {
        public int OrderItemId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }

    public class PaymentDto
    {
        public int OrderId { get; set; }
        public string PaymentMethod { get; set; } = "Card"; // Card, UPI, COD
        public string? CardNumber { get; set; }
        public string? CardExpiry { get; set; }
        public string? CardCvv { get; set; }
    }
}