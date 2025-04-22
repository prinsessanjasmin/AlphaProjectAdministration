using Data.Contexts;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

public class NotificationDismissedRepository(DataContext context) : BaseRepository<NotificationDismissedEntity>(context), INotificationDismissedRepository
{
    private readonly DataContext _context = context;
}
