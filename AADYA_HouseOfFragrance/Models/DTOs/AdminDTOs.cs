using System.ComponentModel.DataAnnotations;

namespace AADYA_HouseOfFragrance.Models.DTOs
{
    public class StatusUpdateDto
    {
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
    }

    public class AddProductDto
    {
        [Required(ErrorMessage = "Product name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Scent family is required.")]
        public string ScentFamily { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        [Range(1, 100000, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price6ml { get; set; }

        [Range(1, 100000, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price12ml { get; set; }
    }

    public class StockUpdateDto
    {
        public int StockQuantity { get; set; }
    }
}