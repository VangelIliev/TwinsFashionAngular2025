namespace TwinsFashionApi.Models
{
    public class ShoppingBasketViewModel
    {
        public ShoppingBasketViewModel()
        {
            Products = new Dictionary<Guid, List<ProductViewModel>>();
            Email = new EmailViewModel();
        }

        public Dictionary<Guid, List<ProductViewModel>> Products { get; set; }
        public EmailViewModel Email { get; set; }
    }
}
