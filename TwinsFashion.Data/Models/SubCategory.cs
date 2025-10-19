using System;
using System.ComponentModel.DataAnnotations;

namespace TwinsFashion.Data.Models
{
    public class SubCategory
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
