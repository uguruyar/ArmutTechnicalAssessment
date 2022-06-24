using Armut.Messaging.SharedKernel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;
using System.Security.Claims;
using System.Text.Json;

namespace Armut.Messaging.Infrastructure.Hubs
{
    //[Authorize]
    [SignalRHub]
    public class ChatHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var userName = Context.User.FindFirstValue(ClaimTypes.Name);
            //var user = Context.User.Identity.Name;

            ConnectionMapping.Add(userName, Context.ConnectionId);

            _ = Task.CompletedTask;

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userName = Context.User.FindFirstValue(ClaimTypes.Name);

            ConnectionMapping.Remove(userName);

            _ = Task.CompletedTask;

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageAsync(string data, CancellationToken cancellationToken = default)
        {
            var jsonData = JsonSerializer.Deserialize<MessageData>(data);

            var targetUserConnectionIds = ConnectionMapping.GetConnections(jsonData.TargetUserName);

            //await Clients.Clients(targetUserConnectionIds).SendAsync("ChatChannel", jsonData.Message);

            await Clients.User(jsonData.TargetUserName).SendAsync("ChatChannel", jsonData.Message);
        }
    }
}
