namespace AADYA_HouseOfFragrance.Models.DTOs
{
    /// <summary>
    /// DTO for adding an item to the shopping cart.
    /// </summary>
    public class AddToCartDto
    {
        /// <summary>
        /// The ID of the product variant to add.
        /// </summary>
        public int VariantId { get; set; }

        /// <summary>
        /// The quantity to add. Defaults to 1.
        /// </summary>
        public int Quantity { get; set; } = 1;
    }

    /// <summary>
    /// DTO for updating an existing cart item's quantity.
    /// </summary>
    public class UpdateCartDto
    {
        /// <summary>
        /// The ID of the cart item to update.
        /// </summary>
        public int CartItemId { get; set; }

        /// <summary>
        /// The new quantity for the cart item.
        /// </summary>
        public int Quantity { get; set; }
    }

    /// <summary>
    /// Response DTO representing a cart item with full product details and calculated totals.
    /// </summary>
    public class CartItemResponseDto
    {
        /// <summary>
        /// The ID of the cart item.
        /// </summary>
        public int CartItemId { get; set; }

        /// <summary>
        /// The ID of the product variant.
        /// </summary>
        public int VariantId { get; set; }

        /// <summary>
        /// The name of the product.
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// The image URL of the product.
        /// </summary>
        public string ProductImage { get; set; } = string.Empty;

        /// <summary>
        /// The size of the variant (e.g., "6ml" or "12ml").
        /// </summary>
        public string Size { get; set; } = string.Empty;

        /// <summary>
        /// The unit price of the variant.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The quantity selected by the user.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The total price for this cart item (Price × Quantity).
        /// </summary>
        public decimal Total { get; set; }
    }
}