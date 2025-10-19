using Microsoft.AspNetCore.Mvc;

namespace TwinsFashionApi.Controllers
{
    public class ShoppingBasketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
