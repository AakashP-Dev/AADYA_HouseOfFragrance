using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AADYA_HouseOfFragrance.Models.Entities
{
    /// <summary>
    /// Represents a fragrance product in the catalog.
    /// Each product belongs to a category and can have multiple variants (sizes).
    /// </summary>
    public class Product
    {
        /// <summary>
        /// The unique identifier for the product.
        /// </summary>
        [Key]
        public int ProductId { get; set; }

        /// <summary>
        /// The ID of the category this product belongs to.
        /// Foreign key to the Category entity.
        /// </summary>
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        /// <summary>
        /// The name of the fragrance product.
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
        /// The scent family of the fragrance.
        /// Possible values: "Woody", "Floral", "Citrus", "Spicy", "Fresh".
        /// </summary>
        public string ScentFamily { get; set; } = string.Empty;

        /// <summary>
        /// The recommended occasion for the fragrance.
        /// Possible values: "Daily", "Evening", "Spiritual".
        /// </summary>
        public string RecommendedOccasion { get; set; } = string.Empty;

        /// <summary>
        /// The intensity level of the fragrance.
        /// Possible values: "Subtle" or "Bold".
        /// </summary>
        public string Intensity { get; set; } = string.Empty;

        /// <summary>
        /// The date and time when the product was added to the catalog (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ============================================
        // NAVIGATION PROPERTIES
        // ============================================

        /// <summary>
        /// The category that this product belongs to.
        /// Represents a many-to-one relationship (many Products → one Category).
        /// </summary>
        public Category? Category { get; set; }

        /// <summary>
        /// The collection of variants (sizes) available for this product.
        /// Represents a one-to-many relationship (one Product → many ProductVariants).
        /// </summary>
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }
}