using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class NotificationEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [ForeignKey(nameof(TargetGroup))]
    public int TargetGroupId { get; set; }
    public TargetGroupEntity TargetGroup { get; set; } = null!;
    public string NotificationType { get; set; } = null!; 
    public string Icon { get; set; } = null!; 
    public string Message { get; set; } = null!;
    public DateTime Created { get; set; } = DateTime.Now;

    public ICollection<NotificationDismissedEntity> DismissedNotifications { get; set; } = []; 
}
