using System;

namespace TwinsFashionApi.Models
{
    public class AdminUpdateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Price { get; set; }
        public int Quantity { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SubCategoryId { get; set; }
        public Guid ColorId { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public List<Guid> SizeIds { get; set; } = new();
    }
}
