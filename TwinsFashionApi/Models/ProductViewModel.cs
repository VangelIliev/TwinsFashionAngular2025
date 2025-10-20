namespace TwinsFashionApi.Models
{
    public class ProductViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new();
        public string CoverImageUrl { get; set; } = string.Empty;
        public List<string> Sizes { get; set; } = new();
        public string SelectedSize { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public string Subcategory { get; set; } = string.Empty;
    }
}
