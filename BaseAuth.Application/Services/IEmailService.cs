using BaseAuth.Domain.Common;
using System.Threading.Tasks;

namespace BaseAuth.Application.Services
{
    public interface IEmailService
    {
        Task<Result> SendEmailAsync(string to, string subject, string body);
        Task<Result> SendEmailConfirmationAsync(string email, string confirmationLink);
        Task<Result> SendPasswordResetAsync(string email, string resetLink);
        Task<Result> SendWelcomeEmailAsync(string email, string userName);
    }
} 