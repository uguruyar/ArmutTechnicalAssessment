using Armut.Messaging.Application.DTOs;
using Armut.Messaging.Application.Services.Abstract;
using Armut.Messaging.Domain.Models;
using Armut.Messaging.SharedKernel.Constants;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using System.Security.Claims;

namespace Armut.Messaging.Application.Services.Concrete
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Account> _account;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IMongoDatabase database, IHttpContextAccessor httpContextAccessor)
        {
            _users = database.GetCollection<User>(DatabaseConstants.CollectionNames.UserCollectionName);
            _account = database.GetCollection<Account>(DatabaseConstants.CollectionNames.AccountCollectionName);
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task BlockUserAsync(string blockedUsername, CancellationToken cancellationToken = default)
        {
            var isBlockedUsernameExists = await _account.Find(x => x.Username == blockedUsername).AnyAsync(cancellationToken);

            if (!isBlockedUsernameExists)
            {
                throw new InvalidOperationException(ErrorMessages.UserNotExists);
            }

            var currentUserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Sid);
            var currentUserName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

            var isUserExists = await _users.Find(x => x.CurrentUser == currentUserName).AnyAsync(cancellationToken);

            if (!isUserExists)
            {
                var user = new User
                {
                    CurrentUser = currentUserName,
                    BlockedUserNames = new List<string> { }
                };
                user.BlockedUserNames.Add(blockedUsername);

                await _users.InsertOneAsync(user, new InsertOneOptions { }, cancellationToken);

                return;
            }

            var currentUser = (await _users.FindAsync(x => x.CurrentUser == currentUserName)).FirstOrDefault();

            var blockedUserNameInList = currentUser.BlockedUserNames.Contains(blockedUsername);

            if (!blockedUserNameInList)
            {
                currentUser.BlockedUserNames.Add(currentUserName);
                var updateResult = await _users.ReplaceOneAsync(a => a.Id.Equals(a.Id), currentUser);
            }
        }
    }
}
