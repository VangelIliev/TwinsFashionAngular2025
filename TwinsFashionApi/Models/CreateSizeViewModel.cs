using System.ComponentModel.DataAnnotations;

namespace TwinsFashionApi.Models
{
    public class CreateSizeViewModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(30)]
        public string Type { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        [MaxLength(30)]
        public string Size { get; set; } = string.Empty;
    }
}
