using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("SendAction", Context.User.Identity.Name, "joined");
        }

        public async Task OnDisconnectedAsync(Exception ex)
        {
            await Clients.All.SendAsync("SendAction", Context.User.Identity.Name, "left");
        }

        public async Task Send(string message)
        {
            await Clients.All.SendAsync("SendMessage", Context.User.Identity.Name, message);
        }
    }
}
