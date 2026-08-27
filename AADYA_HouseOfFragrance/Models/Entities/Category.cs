using System.ComponentModel.DataAnnotations;

namespace AADYA_HouseOfFragrance.Models.Entities
{
    /// <summary>
    /// Represents a product category (e.g., "Signature Blends", "Attars & Oud", "Contemporary Fragrances").
    /// Each category can have multiple products associated with it.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// The unique identifier for the category.
        /// </summary>
        [Key]
        public int CategoryId { get; set; }

        /// <summary>
        /// The name of the category.
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;

        // ============================================
        // NAVIGATION PROPERTIES
        // ============================================

        /// <summary>
        /// The collection of products belonging to this category.
        /// Represents a one-to-many relationship (one Category → many Products).
        /// </summary>
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}