using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Hub
{
    public class Notification  {
        public async Task SendMessage(string user, string message)
{
    //await Clients.All.SendAsync("ReceiveMessage", user, message);
}
     }
}
