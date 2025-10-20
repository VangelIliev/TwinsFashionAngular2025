using System.ComponentModel.DataAnnotations;

namespace TwinsFashionApi.Models
{
    public class EmailViewModel
    {
        [Required(ErrorMessage = "Полето е задължително.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Името трябва да е между 5 и 50 символа.")]
        [RegularExpression(@"^[А-Яа-яA-Za-z\s]{5,50}$", ErrorMessage = "Името не трябва да съдържа цифри или специални символи.")]
        public string FirstAndLastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Полето е задължително.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Градът трябва да е между 3 и 50 символа.")]
        [RegularExpression(@"^[А-Яа-яA-Za-z\s]{3,50}$", ErrorMessage = "Градът не трябва да съдържа цифри или специални символи.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Полето е задължително.")]
        public string ShippingMethod { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Адресът трябва да е до 100 символа.")]
        [RegularExpression(@"^[А-Яа-яA-Za-z0-9\s,.-]{3,100}$", ErrorMessage = "Адресът не трябва да съдържа специални символи (освен , . -).")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Полето е задължително.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Телефонът трябва да съдържа само цифри.")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
