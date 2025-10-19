using System;

namespace TwinsFashion.Domain.Models
{
    public class ColorDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
