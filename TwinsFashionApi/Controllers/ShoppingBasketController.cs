using Microsoft.AspNetCore.Mvc;
using TwinsFashionApi.Models;
using TwinsFashionApi.Services;

namespace TwinsFashionApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingBasketController : ControllerBase
    {
        private readonly ILogger<ShoppingBasketController> _logger;
        private readonly IEmailSender _emailSender;
        public ShoppingBasketController(ILogger<ShoppingBasketController> logger, IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        [HttpPost("order")]
        public async Task<IActionResult> Order([FromBody] ShoppingBasketOrderViewModel order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var products = order.Items.Select(item => new ProductViewModel
            {
                Id = Guid.TryParse(item.ProductId, out var id) ? id : Guid.Empty,
                Name = item.Title,
                Description = string.Empty,
                Price = item.Price,
                Quantity = item.Quantity,
                SelectedSize = item.Size ?? string.Empty,
                Sizes = new List<string>(),
                ImageUrls = new List<string>(),
                Category = string.Empty,
                Color = string.Empty,
                Subcategory = string.Empty
            }).ToList();

            var emailModel = new EmailViewModel
            {
                FirstAndLastName = order.CustomerName,
                City = order.City,
                ShippingMethod = order.DeliveryPlace,
                Address = order.DeliveryAddress,
                PhoneNumber = order.Phone
            };

            try
            {
                await _emailSender.SendEmailAsync(emailModel, products);
                return Ok(new { message = "success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending order email");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Грешка при изпращане на поръчката." });
            }
        }
    }
}
