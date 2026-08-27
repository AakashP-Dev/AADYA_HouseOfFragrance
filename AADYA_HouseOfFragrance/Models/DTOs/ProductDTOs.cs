namespace AADYA_HouseOfFragrance.Models.DTOs
{
    /// <summary>
    /// Response DTO representing a product with all its details and variants.
    /// </summary>
    public class ProductResponseDto
    {
        /// <summary>
        /// The unique ID of the product.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// The name of the product/fragrance.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The detailed description of the fragrance.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The URL or Base64 string for the product image.
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// The scent family of the fragrance (e.g., "Woody", "Floral", "Citrus", "Spicy").
        /// </summary>
        public string ScentFamily { get; set; } = string.Empty;

        /// <summary>
        /// The recommended occasion for the fragrance (e.g., "Daily", "Evening", "Spiritual").
        /// </summary>
        public string RecommendedOccasion { get; set; } = string.Empty;

        /// <summary>
        /// The intensity level of the fragrance ("Subtle" or "Bold").
        /// </summary>
        public string Intensity { get; set; } = string.Empty;

        /// <summary>
        /// The name of the category the product belongs to.
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// The list of variants (sizes) available for this product.
        /// </summary>
        public List<VariantDto> Variants { get; set; } = new List<VariantDto>();
    }

    /// <summary>
    /// DTO representing a single product variant (size, price, and stock).
    /// </summary>
    public class VariantDto
    {
        /// <summary>
        /// The unique ID of the variant.
        /// </summary>
        public int VariantId { get; set; }

        /// <summary>
        /// The size of the variant (e.g., "6ml" or "12ml").
        /// </summary>
        public string Size { get; set; } = string.Empty;

        /// <summary>
        /// The price of the variant.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The current stock quantity available for this variant.
        /// </summary>
        public int StockQuantity { get; set; }
    }

    /// <summary>
    /// DTO for the Fragrance Wizard request.
    /// </summary>
    public class WizardRequestDto
    {
        /// <summary>
        /// The selected scent family (e.g., "Woody", "Floral", "Citrus", "Spicy").
        /// </summary>
        public string ScentFamily { get; set; } = string.Empty;

        /// <summary>
        /// The selected occasion (e.g., "Daily", "Evening", "Spiritual").
        /// </summary>
        public string Occasion { get; set; } = string.Empty;

        /// <summary>
        /// The selected intensity ("Subtle" or "Bold").
        /// </summary>
        public string Intensity { get; set; } = string.Empty;
    }
}