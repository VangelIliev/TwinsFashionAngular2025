using System.ComponentModel.DataAnnotations;

namespace TwinsFashionApi.Models
{
    public class CreateCategoryViewModel
    {
        [Required]
        [MinLength(5)]
        [MaxLength(15)]
        public string Name { get; set; } = string.Empty;
    }
}
