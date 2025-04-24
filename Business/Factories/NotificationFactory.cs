using Data.Entities;

namespace Business.Factories;

public static class NotificationFactory
{
    public static NotificationEntity Create(int targetGroupId, int notificationTypeId, string message, string icon)
    {
        var notification = new NotificationEntity
        {
            TargetGroupId = targetGroupId,
            NotificationTypeId = notificationTypeId,
            Icon = icon,
            Message = message
        };

        return notification;
    }
}
