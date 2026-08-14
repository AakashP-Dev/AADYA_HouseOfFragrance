namespace AADYA_HouseOfFragrance.Models.DTOs
{
    public class AddToCartDto
    {
        public int VariantId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartDto
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class CartItemResponseDto
    {
        public int CartItemId { get; set; }
        public int VariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}