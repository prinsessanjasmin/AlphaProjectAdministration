using Data.Entities;

namespace WebApp_MVC.Models;

public class ProjectDetailsViewModel()
{
    public int Id { get; set; } 
    public string? ProjectImagePath { get; set; }
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public string Client { get; set; } = null!;
    public string Status { get; set; } = null!;

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal? Budget {  get; set; }
    public List<ProjectEmployeeEntity> TeamMembers { get; set; } = null!;
}
