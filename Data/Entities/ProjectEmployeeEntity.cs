using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class ProjectEmployeeEntity
{
    

    public int ProjectId { get; set; }
    public ProjectEntity Project { get; set; } = null!;



    public string EmployeeId { get; set; } = null!;
    public ApplicationUser Employee { get; set; } = null!;

}
