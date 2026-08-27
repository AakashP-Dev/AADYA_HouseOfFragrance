using System.ComponentModel.DataAnnotations;

namespace AADYA_HouseOfFragrance.Models.DTOs
{
    /// <summary>
    /// DTO for updating the status of an order, with an optional reason (e.g., for rejection).
    /// </summary>
    public class StatusUpdateDto
    {
        /// <summary>
        /// The new status to set (e.g., "Processing", "Dispatched", "Delivered", "Cancelled").
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Optional reason for the status change (used when cancelling or rejecting).
        /// </summary>
        public string? Reason { get; set; }
    }

    /// <summary>
    /// DTO for adding a new product to the catalog.
    /// All required fields are validated via Data Annotations.
    /// </summary>
    public class AddProductDto
    {
        /// <summary>
        /// Product name (required).
        /// </summary>
        [Required(ErrorMessage = "Product name is required.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Category name (must match an existing category).
        /// </summary>
        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Scent family (e.g., Woody, Floral, Citrus).
        /// </summary>
        [Required(ErrorMessage = "Scent family is required.")]
        public string ScentFamily { get; set; } = string.Empty;

        /// <summary>
        /// Product description (required).
        /// </summary>
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Optional image URL or Base64 string.
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Price for the 6ml variant (must be greater than 0).
        /// </summary>
        [Range(1, 100000, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price6ml { get; set; }

        /// <summary>
        /// Price for the 12ml variant (must be greater than 0).
        /// </summary>
        [Range(1, 100000, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price12ml { get; set; }
    }

    /// <summary>
    /// DTO for updating stock quantities for both 6ml and 12ml variants.
    /// </summary>
    public class StockUpdateDto
    {
        /// <summary>
        /// Stock quantity for the 6ml variant.
        /// </summary>
        public int Stock6ml { get; set; }

        /// <summary>
        /// Stock quantity for the 12ml variant.
        /// </summary>
        public int Stock12ml { get; set; }
    }
}