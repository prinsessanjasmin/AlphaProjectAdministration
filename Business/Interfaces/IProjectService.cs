using Data.Entities;
using Business.Models;

namespace Business.Interfaces;

public interface IProjectService
{
    Task<IResult> CreateProject(ProjectDto form);
    Task<IResult> GetAllProjects();
    Task<IResult> GetProjectById(int id);
    Task<IResult> GetProjectByProjectName(string projectName);
    Task<IResult> GetProjectsByStartdate(DateTime startDate);
    Task<IResult> GetProjectsByEndDate(DateTime endDate);
    Task<IResult> UpdateProject(int id, ProjectEntity updatedProject);
    Task<IResult> DeleteProject(int id);
}
