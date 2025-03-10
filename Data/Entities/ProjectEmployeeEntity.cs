using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class ProjectEmployeeEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProjectId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

}
