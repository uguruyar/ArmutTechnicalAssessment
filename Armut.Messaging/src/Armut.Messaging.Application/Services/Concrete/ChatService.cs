using Armut.Messaging.Application.Services.Abstract;
using Armut.Messaging.Domain.Models;
using Armut.Messaging.Infrastructure.Hubs;
using Armut.Messaging.SharedKernel.Constants;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace Armut.Messaging.Application.Services.Concrete
{
    public class ChatService : IChatService
    {
        private readonly IMongoCollection<UserMessage> _userMessages;
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly IMongoCollection<User> _users;

        public ChatService(IMongoDatabase database, IHubContext<ChatHub> chatHub)
        {
            _userMessages = database.GetCollection<UserMessage>(DatabaseConstants.CollectionNames.UserMessagesCollectionName);
            _users = database.GetCollection<User>(DatabaseConstants.CollectionNames.UserCollectionName);
            _chatHub = chatHub;
        }

        public async Task SendMessageAsync(string sourceUserName, string targetUserName, string message, CancellationToken cancellationToken = default)
        {
            var isUserBlocked = await _users.Find(x => x.CurrentUser == sourceUserName && x.BlockedUserNames.Contains(targetUserName)).AnyAsync(cancellationToken);

            if (isUserBlocked)
            {
                throw new InvalidOperationException(ErrorMessages.UserBlocked);
            }

            var targetUserConnectionIds = ConnectionMapping.GetConnections(targetUserName);
            await _chatHub.Clients.Clients(targetUserConnectionIds).SendAsync("SendMessageAsync", targetUserName, message, cancellationToken);
            await AddChatHistoryAsync(sourceUserName, targetUserName, message);
        }

        private async Task AddChatHistoryAsync(string sourceUserName, string targetUserName, string message, CancellationToken cancellationToken = default)
        {
            await _userMessages.InsertOneAsync(new UserMessage
            {
                SourceUserName = sourceUserName.ToLowerInvariant(),
                TargetUserName = targetUserName.ToLowerInvariant(),
                Message = message,
            }, new InsertOneOptions { }, cancellationToken);
        }
    }
}
