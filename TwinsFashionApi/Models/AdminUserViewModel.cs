using System.ComponentModel.DataAnnotations;

namespace TwinsFashionApi.Models
{
    public class AdminUserViewModel
    {
        [Required]
        [MinLength(10)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(10)]
        public string Password { get; set; } = string.Empty;
    }
}
