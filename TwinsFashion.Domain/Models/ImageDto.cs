using System;

namespace TwinsFashion.Domain.Models
{
    public class ImageDto
    {
        public Guid Id { get; set; }
        public required string Url { get; set; }
    }
}
