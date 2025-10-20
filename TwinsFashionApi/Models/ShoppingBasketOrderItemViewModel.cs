using System.ComponentModel.DataAnnotations;

namespace TwinsFashionApi.Models
{
    public class ShoppingBasketOrderItemViewModel
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        public string? Size { get; set; }
    }
}

