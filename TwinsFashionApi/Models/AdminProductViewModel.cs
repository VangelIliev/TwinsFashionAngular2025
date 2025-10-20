namespace TwinsFashionApi.Models
{
    public class AdminProductViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public List<AdminImageViewModel> Images { get; set; } = new();
        public string CoverImageUrl { get; set; } = string.Empty;
        public List<string> Sizes { get; set; } = new();
        public int Quantity { get; set; } = 1;
        public string Subcategory { get; set; } = string.Empty;
    }

    public class AdminImageViewModel
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool IsCover { get; set; }
    }
}
