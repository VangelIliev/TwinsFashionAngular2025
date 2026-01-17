using System;
using System.ComponentModel.DataAnnotations;

namespace TwinsFashionApi.Models
{
    public class AdminAddProductRequest
    {
        [Required]
        [MinLength(5)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MinLength(10)]
        public string Description { get; set; } = string.Empty;

        [Range(0, 1000000)]
        public int Price { get; set; }

        [Range(0, 1000000)]
        public int Quantity { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public Guid SubCategoryId { get; set; }

        [Required]
        public Guid ColorId { get; set; }

        [MaxLength(50)]
        public List<string> ImageUrls { get; set; } = new();

        [MaxLength(100)]
        public List<Guid> SizeIds { get; set; } = new();
    }
}
