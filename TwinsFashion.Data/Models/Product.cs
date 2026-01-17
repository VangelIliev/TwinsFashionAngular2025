using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TwinsFashion.Data.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }

        [MinLength(5)]
        public required string Name { get; set; }

        [Range(0, 1000)]
        public int Price { get; set; }

        [Range(0, 1000)]
        public int Quantity { get; set; }

        [MinLength(10)]
        public required string Description { get; set; }

        public Guid CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        public Guid SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; } = null!;

        public Guid ColorId { get; set; }

        public Color Color { get; set; } = null!;
        public ICollection<Image> Images { get; set; } = new List<Image>();

        public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

        public ICollection<Size> Sizes { get; set; } = new List<Size>();
    }
}
