using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Business.Interfaces;
using Microsoft.AspNetCore.SignalR;
using WebApp_MVC.Hubs;
using Data.Entities;
using System.Security.Claims;

namespace WebApp_MVC.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController(IHubContext<NotificationHub> notificationHub, INotificationService notificationService) : ControllerBase
{
    private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;
    private readonly INotificationService _notificationService = notificationService;

    [HttpPost]
    public async Task<IActionResult> CreateNotification(NotificationEntity notification, string userId)
    {
        await _notificationService.AddNotificationAsync(notification);
        var notifications = await _notificationService.GetAllAsync(userId); 
        var newNotification = notifications.OrderByDescending(x => x.Created).FirstOrDefault();

        if (newNotification != null)
        {
            await _notificationHub.Clients.All.SendAsync("ReceiveNotification", newNotification);
        }
        return Ok(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous"; 
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var notifications = await _notificationService.GetAllAsync(userId); 
        return Ok(notifications);
    }

    [HttpPost("dismiss/{id}")] 
    public async Task<IActionResult> DismissNotification(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        await _notificationService.DismissNotificationAsync(id, userId);
        await _notificationHub.Clients.User(userId).SendAsync("NotificationDismissed", id);
        return Ok(new { success = true });
    }
}
