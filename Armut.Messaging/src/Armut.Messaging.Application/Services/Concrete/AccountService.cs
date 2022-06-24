using Armut.Messaging.Application.DTOs;
using Armut.Messaging.Application.Services.Abstract;
using Armut.Messaging.Domain.Models;
using Armut.Messaging.Infrastructure.Services;
using Armut.Messaging.SharedKernel.Constants;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Security.Claims;

namespace Armut.Messaging.Application.Services.Concrete
{
    public class AccountService : IAccountService
    {
        private readonly IMongoCollection<Account> _accounts;
        private readonly IConfiguration _configuration;

        public AccountService(IMongoDatabase database, IConfiguration configuration)
        {
            _accounts = database.GetCollection<Account>(DatabaseConstants.CollectionNames.AccountCollectionName);
            _configuration = configuration;
        }

        public async Task<AuthResponse> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            var hashedPassword = HashPassword(password);

            var account = await _accounts.Find(x => x.Username == username && x.Password == hashedPassword).SingleOrDefaultAsync(cancellationToken);

            if (account == null)
            {
                throw new InvalidOperationException(ErrorMessages.UserNotFound);
            }

            var claims = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Sid, account.Id),
                new Claim(ClaimTypes.Name, account.Username),
            });

            var tokenSecurityKey = _configuration.GetSection("Identity:TokenSecurityKey").Value;

            var token = TokenService.Create(claims, tokenSecurityKey);

            return new AuthResponse(token);
        }

        public async Task RegisterAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            var isUserNameExists = await _accounts.Find(x => x.Username.ToLower() == username.ToLower()).AnyAsync(cancellationToken);

            if (isUserNameExists)
            {
                throw new InvalidOperationException(ErrorMessages.UsernameAlreadyInUse);
            }

            var hashedPassword = HashPassword(password);

            var account = new Account(username, hashedPassword);

            await _accounts.InsertOneAsync(account, new InsertOneOptions { }, cancellationToken);
        }

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}
