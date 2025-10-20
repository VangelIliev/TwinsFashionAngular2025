using System.ComponentModel.DataAnnotations;

namespace TwinsFashionApi.Models
{
    public class AdminAddProductViewModel
    {
        [Required, MinLength(3), MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required, MinLength(10), MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [Range(0, 100000)]
        public int Price { get; set; }

        [Range(0, 100000)]
        public int Quantity { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public Guid SubCategoryId { get; set; }

        [Required]
        public Guid ColorId { get; set; }

        // Optional: URLs (one per line) as fallback
        public string ImageUrlsText { get; set; } = string.Empty;

        // Preferred: uploaded images from device
        public List<IFormFile> UploadedImages { get; set; } = new();

        [Required]
        public List<Guid> SelectedSizeIds { get; set; } = new();

        // Lookup lists
        public IEnumerable<CategoryViewModel> Categories { get; set; } = Enumerable.Empty<CategoryViewModel>();
        public IEnumerable<SubCategoryViewModel> SubCategories { get; set; } = Enumerable.Empty<SubCategoryViewModel>();
        public IEnumerable<ColorViewModel> Colors { get; set; } = Enumerable.Empty<ColorViewModel>();
        public IEnumerable<SizeViewModel> Sizes { get; set; } = Enumerable.Empty<SizeViewModel>();
    }
}
