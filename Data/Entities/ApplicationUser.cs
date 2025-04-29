using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class ApplicationUser : IdentityUser
{
    [Required]
    [PersonalData]
    [Column(TypeName = "nvarchar(50)")]
    public string FirstName { get; set; } = null!;

    [Required]
    [PersonalData]
    [Column(TypeName = "nvarchar(50)")]
    public string LastName { get; set; } = null!;
 
    [Column(TypeName = "nvarchar(50)")]
    public string? JobTitle { get; set; }

    public string? ProfileImagePath { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? DateOfBirth { get; set; }

    public ICollection<ProjectEmployeeEntity> EmployeeProjects { get; set; } = [];

    public int? AddressId { get; set; }
    public AddressEntity Address { get; set; } = null!;

    public ICollection<NotificationDismissedEntity> DismissedNotifications { get; set; } = [];

    public bool IsProfileComplete { get; set; } = false;
}
