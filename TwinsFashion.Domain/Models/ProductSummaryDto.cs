using System;

namespace TwinsFashion.Domain.Models
{
    public class ProductSummaryDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public string Badge { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;
    }
}


