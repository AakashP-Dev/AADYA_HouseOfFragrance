using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AADYA_HouseOfFragrance.Models.Entities
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string ScentFamily { get; set; } = string.Empty;
        public string RecommendedOccasion { get; set; } = string.Empty;
        public string Intensity { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public Category Category { get; set; } = new Category();
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }
}