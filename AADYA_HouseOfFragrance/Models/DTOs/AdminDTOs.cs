namespace AADYA_HouseOfFragrance.Models.DTOs
{
    public class StatusUpdateDto
    {
        public string Status { get; set; } = string.Empty;
    }

    public class AddProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ScentFamily { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price6ml { get; set; }
        public decimal Price12ml { get; set; }
    }
}