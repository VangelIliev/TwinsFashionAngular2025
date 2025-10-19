using System;
using System.ComponentModel.DataAnnotations;

namespace TwinsFashion.Data.Models
{
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [MaxLength(200)]
        public string AddressForOrder { get; set; } = string.Empty;
    }
}
