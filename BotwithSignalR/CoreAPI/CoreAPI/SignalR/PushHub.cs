using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI.SignalR
{
    /// <summary>
    /// Setting SignalR as a service
    /// https://stackoverflow.com/questions/46904678/call-signalr-core-hub-method-from-controller/46906849
    /// </summary>
    [AllowAnonymous]
    //[AllowAnonymous]
    public class PushHub : Hub
    {
        public Task SendMessageToUser(string toUser, string fromUser, string message)
        {
            return Clients.All.SendAsync(toUser, fromUser, message);
        }

        public Task SendMessage(string user, string message)
        {
            return Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public Task SendMessageToCaller(string message)
        {
            return Clients.Caller.SendAsync("ReceiveMessage", message);
        }

        public Task SendMessageToGroup(string message)
        {
            return Clients.Group("SignalR Users").SendAsync("ReceiveMessage", message);
        }

    }
}
