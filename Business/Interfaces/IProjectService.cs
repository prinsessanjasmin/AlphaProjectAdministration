using Data.Entities;
using Business.Models;

namespace Business.Interfaces;

public interface IProjectService
{
    Task<IResult> CreateProject(ProjectFormModel form);
    Task<IResult> GetAllProjects();
    Task<IResult> GetProjectById(int id);
    Task<IResult> GetProjectByProjectName(string projectName);
    Task<IResult> GetProjectByStartdate(DateTime startDate);
    Task<IResult> GetProjectByEndDate(DateTime endDate);
    Task<IResult> UpdateProject(int id, ProjectEntity updatedProject);
    Task<IResult> DeleteProject(int id);
}
