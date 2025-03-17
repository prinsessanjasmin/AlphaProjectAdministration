using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class EmployeeEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string FirstName { get; set; } = null!;

    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string LastName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [Column(TypeName = "varchar(20)")]
    public string PhoneNumber { get; set; } = null!;

    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string JobTitle { get; set; } = null!;

    public IFormFile? ProfileImage { get; set; }

    [DataType(DataType.Date)]   
    public DateOnly DateOfBirth { get; set; }

    public ICollection<ProjectEmployeeEntity> EmployeeProjects { get; set; } = new List<ProjectEmployeeEntity>();

    [Required]
    public int AddressId { get; set; }
    public AddressEntity Address { get; set; } = null!; 
}
