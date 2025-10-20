using System.ComponentModel.DataAnnotations;

namespace TwinsFashionApi.Models
{
    public class CreateSubCategoryViewModel
    {
        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        // For dropdown
        public IEnumerable<CategoryViewModel> Categories { get; set; } = Enumerable.Empty<CategoryViewModel>();
    }
}
