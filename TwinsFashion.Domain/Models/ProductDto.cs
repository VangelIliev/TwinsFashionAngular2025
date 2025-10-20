using System;
using System.Collections.Generic;

namespace TwinsFashion.Domain.Models
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string LongDescription { get; set; } = string.Empty;
        public int Price { get; set; }
        public int Quantity { get; set; }
        public string Badge { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;
        public CategoryDto? Category { get; set; }
        public SubCategoryDto? SubCategory { get; set; }
        public ColorDto? Color { get; set; }
        public IList<ImageDto> Images { get; set; } = new List<ImageDto>();
        public IList<SizeDto> Sizes { get; set; } = new List<SizeDto>();
    }
}
