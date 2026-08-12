using System.ComponentModel.DataAnnotations;

namespace AADYA_HouseOfFragrance.Models.Entities
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        // Navigation Property
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}