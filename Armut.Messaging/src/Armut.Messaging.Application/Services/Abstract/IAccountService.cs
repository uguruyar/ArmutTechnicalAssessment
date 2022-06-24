using Armut.Messaging.Application.DTOs;

namespace Armut.Messaging.Application.Services.Abstract
{
    public interface IAccountService
    {
        Task<AuthResponse> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default);

        Task RegisterAsync(string username, string password, CancellationToken cancellationToken = default);
    }
}
