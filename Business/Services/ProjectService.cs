using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public class ProjectService(IProjectRepository projectRepository) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;

    public async Task<IResult> CreateProject(ProjectFormModel form)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> DeleteProject(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> GetAllProjects()
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> GetProjectByEndDate(DateTime endDate)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> GetProjectById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> GetProjectByProjectName(string projectName)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> GetProjectByStartdate(DateTime startDate)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> UpdateProject(int id, ProjectEntity updatedProject)
    {
        throw new NotImplementedException();
    }


    //From Claude AI, suggestion about how to handle the projectemployees in an update scenario:
    //public async Task UpdateProject(ProjectFormModel formModel)
    //{
    //    var existingProject = await _dbContext.Projects
    //        .Include(p => p.ProjectEmployees)
    //        .FirstOrDefaultAsync(p => p.ProjectId == formModel.ProjectId);

    //    if (existingProject != null)
    //    {
    //        // Update basic properties
    //        existingProject.ProjectName = formModel.ProjectName;
    //        // ... other property updates

    //        // Remove existing team members
    //        existingProject.ProjectEmployees.Clear();

    //        // Add selected team members
    //        foreach (var employeeId in formModel.SelectedEmployeeIds)
    //        {
    //            existingProject.ProjectEmployees.Add(new ProjectEmployeeEntity
    //            {
    //                ProjectId = existingProject.ProjectId,
    //                EmployeeId = employeeId
    //            });
    //        }

    //        await _dbContext.SaveChangesAsync();
    //    }
    //}
}
