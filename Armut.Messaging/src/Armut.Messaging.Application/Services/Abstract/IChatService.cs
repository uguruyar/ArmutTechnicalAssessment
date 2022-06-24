using Armut.Messaging.Domain.Models;

namespace Armut.Messaging.Application.Services.Abstract
{
    public interface IChatService
    {
        Task SendMessageAsync(string sourceUserName, string targetUserName, string message, CancellationToken cancellationToken = default);
    }
}
