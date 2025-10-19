using System;
using System.ComponentModel.DataAnnotations;

namespace TwinsFashion.Data.Models
{
    public class Size
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(10)]
        public string Name { get; set; } = string.Empty;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
