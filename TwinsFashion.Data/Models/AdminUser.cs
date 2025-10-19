using System.ComponentModel.DataAnnotations;

namespace TwinsFashion.Data.Models
{
    public class AdminUser
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string PasswordSalt { get; set; } = string.Empty;
    }
}
