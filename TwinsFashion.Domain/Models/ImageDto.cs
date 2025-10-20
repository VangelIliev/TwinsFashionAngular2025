using System;

namespace TwinsFashion.Domain.Models
{
    public class ImageDto
    {
        public Guid Id { get; set; }
        public required string Url { get; set; }
        public string Alt { get; set; } = string.Empty;
        public bool IsCover { get; set; }
    }
}
