using System;

namespace TwinsFashion.Domain.Models
{
    public class SizeDto
    {
        public Guid Id { get; set; }
        // JSON stored in DB is like {"type":"Обувки","size":"39"}
        public required string Size { get; set; }
        public required string Type { get; set; }
    }
}
