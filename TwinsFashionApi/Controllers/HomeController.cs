using Microsoft.AspNetCore.Mvc;
using TwinsFashionApi.Models;
using TwinsFashionApi.Models.Mappings;
using TwinsFashionApi.Services;

namespace TwinsFashionApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;
        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        [HttpPost("contacts")]
        public async Task<IActionResult> Contacts([FromBody] EmailContactUsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _emailSender.SendContactUsEmail(model);
            return Ok(new { message = "success" });
        }
    }
}
