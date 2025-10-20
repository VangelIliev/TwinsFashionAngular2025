using TwinsFashionApi.Models;

namespace TwinsFashionApi.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailViewModel model, IEnumerable<ProductViewModel> products);

        Task SendContactUsEmail(EmailContactUsViewModel model);
    }
}
