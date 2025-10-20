using System.ComponentModel.DataAnnotations;

namespace TwinsFashionApi.Models
{
    public class ShoppingBasketOrderViewModel
    {
        [Required]
        [RegularExpression("^[А-Яа-яA-Za-z\\s]+$", ErrorMessage = "Името не трябва да съдържа цифри или специални символи.")]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^[А-Яа-яA-Za-z0-9\\s]+$", ErrorMessage = "Градът не може да съдържа специални символи.")]
        public string City { get; set; } = string.Empty;

        [Required]
        public string DeliveryPlace { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^[А-Яа-яA-Za-z0-9\\s.,-]+$", ErrorMessage = "Адресът трябва да съдържа само букви, цифри и ,.-")]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(\\+359|0)(87|88|89|98)[0-9]{7}$", ErrorMessage = "Въведете валиден български телефонен номер.")]
        public string Phone { get; set; } = string.Empty;

        public IEnumerable<ShoppingBasketOrderItemViewModel> Items { get; set; } = Array.Empty<ShoppingBasketOrderItemViewModel>();

        public decimal Total { get; set; }
    }
}

