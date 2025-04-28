using Business.Models;
using Business.Interfaces;
using Data.Entities;

namespace WebApp_MVC.Models;

public class ProjectViewModel(IProjectService projectService)
{
    private readonly IProjectService _projectService = projectService;
    public List<ProjectEntity> Projects { get; set; } = [];

    public List<TeamMemberDto> AvailableTeamMembers { get; set; } = [];

    public async Task GetProjects()
    {
        var result = await _projectService.GetAllProjects();

        if (result.Success)
        {
            var projectResult = result as Result<IEnumerable<ProjectEntity>>;
            var projects = projectResult?.Data;

            if (projects != null)
            {
                Projects = projects.ToList();
            }
        }
    }
}
