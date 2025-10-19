using System;
using System.ComponentModel.DataAnnotations;

namespace TwinsFashion.Data.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        [MinLength(5)]
        [MaxLength(15)]
        public required string Name { get; set; }

        public ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
    }
}
