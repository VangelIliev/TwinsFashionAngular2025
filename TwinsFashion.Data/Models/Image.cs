using System;
using System.ComponentModel.DataAnnotations;

namespace TwinsFashion.Data.Models
{
    public class Image
    {
        [Key]
        public Guid Id { get; set; }
        [Url]
        public required string Url { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
