using System.ComponentModel.DataAnnotations;

namespace TwinsFashionApi.Models
{
    public class CreateColorViewModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;
    }
}
