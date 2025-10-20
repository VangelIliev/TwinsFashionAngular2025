using System.ComponentModel.DataAnnotations;

namespace TwinsFashionApi.Models
{
    public class EmailContactUsViewModel
    {
        [Required(ErrorMessage = "Полето е задължително")]
        [EmailAddress(ErrorMessage = "Моля въведете валиден Имейл адрес")]
        public required string EmailAddress { get; set; }

        [Required(ErrorMessage = "Полето е задължително")]
        [MinLength(10, ErrorMessage = "Полето трябва да съдържа минимум 10 символа")]
        [MaxLength(100, ErrorMessage = "Полето трядва да съдържа максимум 100 символа")]
        public required string Description { get; set; }
    }
}
