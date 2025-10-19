using System;

namespace TwinsFashion.Domain.Models
{
    public class SubCategoryDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public Guid CategoryId { get; set; }
        public CategoryDto? Category { get; set; }
    }
}
