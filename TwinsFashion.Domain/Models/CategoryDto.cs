using System;

namespace TwinsFashion.Domain.Models
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
