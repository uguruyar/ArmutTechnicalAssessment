namespace Armut.Messaging.Application.Services.Abstract
{
    public interface IUserService
    {
        Task BlockUserAsync(string username, CancellationToken cancellationToken = default);
    }
}
