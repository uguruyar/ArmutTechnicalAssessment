using Armut.Messaging.Domain.Models;

namespace Armut.Messaging.Application.Services.Abstract
{
    public interface IMessageHistoryService
    {
        Task<List<UserMessage>> GetMessageHistoryAsync(string userName, string targetUserName, CancellationToken cancellationToken = default);
    }
}
