using Business.Interfaces;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;

namespace Business.Services;

public class NotificationService(INotificationRepository notificationRepository, INotificationDismissedRepository notificationDismissedRepository) : INotificationService
{
    private readonly INotificationRepository _notificationRepository = notificationRepository;
    private readonly INotificationDismissedRepository _notificationDismissedRepository = notificationDismissedRepository; 
 
    public async Task AddNotificationAsync(NotificationEntity notification)
    {
        if (string.IsNullOrWhiteSpace(notification.Icon))
        {
            switch (notification.NotificationTypeId)
            {
                case 1:
                    notification.Icon = "/ProjectImages/Icons/avatar2.svg";
                    break;

                case 2:
                    notification.Icon = "/ProjectImages/Icons/projectlogo.svg";
                    break;

                case 3:
                    notification.Icon = "/ProjectImages/Icons/suitcase-blue.svg";
                    break;
            }
        }

        var notificationEntity = new NotificationEntity
        {
            TargetGroupId = notification.TargetGroupId,
            NotificationTypeId = notification.NotificationTypeId,
            Icon = notification.Icon,
            Message = notification.Message,
        };

        await _notificationRepository.CreateAsync(notificationEntity);
        await _notificationRepository.SaveAsync();
    }

    public async Task<IEnumerable<NotificationEntity>> GetAllAsync(string userId, int take = 10)
    {
        var dismissed = await _notificationDismissedRepository.GetAsync();
        var dismissedIds = dismissed.Where(x => x.UserId == userId).Select(x => x.NotificationId).ToList();
        var notifications = await _notificationRepository.GetAsync();

        var userNotifications = notifications
        .Where(notification => !dismissedIds.Contains(notification.Id))
        .OrderByDescending(notification => notification.Created) 
        .Take(take)
        .ToList();

        return userNotifications;
    }

    public async Task DismissNotificationAsync(string notificationId, string userId)
    {
        var dismissed = await _notificationDismissedRepository.GetAsync();
        var alreadyDismissed = dismissed.Any(x => x.NotificationId == notificationId && x.UserId == userId);

        if(!alreadyDismissed)
        {
            var dismissEntity = new NotificationDismissedEntity
            {
                NotificationId = notificationId,
                UserId = userId
            };

            await _notificationDismissedRepository.CreateAsync(dismissEntity);
            await _notificationDismissedRepository.SaveAsync(); 
        }
    }
}
