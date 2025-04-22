using Data.Entities;

namespace Business.Interfaces
{
    public interface INotificationService
    {
        Task AddNotificationAsync(NotificationEntity notification);
        Task<IEnumerable<NotificationEntity>> GetAllAsync(string userId, int take = 10);
        Task DismissNotificationAsync(string notificationId, string userId);
    }
}