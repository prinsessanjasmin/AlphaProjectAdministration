using Microsoft.AspNetCore.SignalR;

namespace WebApp_MVC.Hubs;

public class NotificationHub : Hub
{
    public async Task SendNotificationToAll(object notification) 
    {
        await Clients.All.SendAsync("AllReceiveNotification", notification); 
    }

    public async Task SendNotificationToAdmins(object notification)
    {
        await Clients.All.SendAsync("AdminReceiveNotification", notification);
    }
}
