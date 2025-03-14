using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class ProjectEmployeeEntity
{
    [Required]
    public int ProjectId { get; set; }
    public ProjectEntity Project { get; set; } = null!; 

    [Required]
    public int EmployeeId { get; set; }
    public EmployeeEntity Employee { get; set; } = null!;

}
