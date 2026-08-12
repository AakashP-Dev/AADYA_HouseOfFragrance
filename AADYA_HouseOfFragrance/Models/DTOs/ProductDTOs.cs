namespace AADYA_HouseOfFragrance.Models.DTOs
{
    public class ProductResponseDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string ScentFamily { get; set; } = string.Empty;
        public string RecommendedOccasion { get; set; } = string.Empty;
        public string Intensity { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public List<VariantDto> Variants { get; set; } = new List<VariantDto>();
    }

    public class VariantDto
    {
        public int VariantId { get; set; }
        public string Size { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class WizardRequestDto
    {
        public string ScentFamily { get; set; } = string.Empty;
        public string Occasion { get; set; } = string.Empty;
        public string Intensity { get; set; } = string.Empty;
    }
}