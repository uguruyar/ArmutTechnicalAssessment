using Armut.Messaging.Application.Services.Abstract;
using Armut.Messaging.Domain.Models;
using Armut.Messaging.SharedKernel.Constants;
using MongoDB.Driver;

namespace Armut.Messaging.Application.Services.Concrete
{
    public class MessageHistoryService : IMessageHistoryService
    {
        private readonly IMongoCollection<UserMessage> _userMessages;

        public MessageHistoryService(IMongoDatabase database)
        {
            _userMessages = database.GetCollection<UserMessage>(DatabaseConstants.CollectionNames.UserMessagesCollectionName, new MongoCollectionSettings());
        }

        public async Task<List<UserMessage>> GetMessageHistoryAsync(string userName, string targetUserName, CancellationToken cancellationToken = default)
        {
            var messageHistory = await _userMessages.Find(x => x.SourceUserName == userName.ToLowerInvariant() && x.TargetUserName == targetUserName.ToLowerInvariant()).ToListAsync(cancellationToken);

            return messageHistory.OrderByDescending(x => x.CreatedAt).ToList();
        }
    }
}
