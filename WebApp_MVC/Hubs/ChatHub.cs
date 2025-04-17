using Microsoft.AspNetCore.SignalR;
using System.Runtime.InteropServices;

namespace WebApp_MVC.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string userName, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", userName, message);
    }
}
