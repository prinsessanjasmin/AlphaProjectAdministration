using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public class ProjectService(IProjectRepository projectRepository) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;

    public async Task<IResult> CreateProject(ProjectDto form)
    {
        await _projectRepository.BeginTransactionAsync();
        try
        {
            if (form == null)
            {
                return Result.BadRequest("You need to fill out the form.");
            }

            if (await _projectRepository.AlreadyExistsAsync (x => x.ProjectName == form.ProjectName 
                && x.ClientId == form.ClientId))
            {
                return Result.AlreadyExists("A project with the same name already exists.");
            }

            var entity = ProjectFactory.Create(form);
            ProjectEntity project = await _projectRepository.CreateAsync(entity);
            await _projectRepository.SaveAsync();
            await _projectRepository.CommitTransactionAsync();
            return Result<ProjectEntity>.Created(project);
        }
        catch
        {
            await _projectRepository.RollbackTransactionAsync();
            return Result.Error("Something went wrong.");
        }
    }

    public async Task<IResult> GetAllProjects()
    {
        try
        {
            var projects = await _projectRepository.GetAsync();
            if (projects != null)
            {
                return Result<IEnumerable<ProjectEntity>>.Ok(projects);
            }
            return Result.NotFound("There are no projects.");
            
        }
        catch
        {
            return Result.Error("Something went wrong");
        }
    }

    public async Task<IResult> GetProjectById(int id)
    {
        try
        {
            ProjectEntity project = await _projectRepository.GetAsync(p => p.ProjectId == id);
            if (project == null)
            {
                return Result.NotFound("No project with that id was found.");
            }

            return Result<ProjectEntity>.Ok(project);
        }
        catch
        {
            return Result.Error("Something went wrong");
        }
    }

    public async Task<IResult> GetProjectByProjectName(string projectName)
    {
        try
        {
            ProjectEntity project = await _projectRepository.GetAsync(p => p.ProjectName == projectName);
            if (project == null)
            {
                return Result.NotFound("No project with that name was found.");
            }

            return Result<ProjectEntity>.Ok(project);
        }
        catch
        {
            return Result.Error("Something went wrong");
        }
    }


    public async Task<IResult> GetProjectsByStartdate(DateTime startDate)
    {
        IEnumerable<ProjectEntity> projects = new List<ProjectEntity>();
        List<ProjectEntity> matchingProjects = new List<ProjectEntity>();
        try
        {
            projects = await _projectRepository.GetAsync();
            if (projects == null)
            {
                return Result.NotFound("There are no projects registered.");
            }
            else
            {
                foreach (var entity in projects)
                {
                    if (entity.EndDate.Equals(startDate))
                    {
                        matchingProjects.Add(entity);
                    }
                }

                if (matchingProjects.Count == 0)
                {
                    return Result.NotFound("No projects with this start date exists.");
                }
                else
                {
                    return Result<List<ProjectEntity>>.Ok(matchingProjects);
                }
            }
        }
        catch
        {
            return Result.Error("Something went wrong");
        }
    }

    public async Task<IResult> GetProjectsByEndDate(DateTime endDate)
    {
        IEnumerable<ProjectEntity> projects = new List<ProjectEntity>();
        List<ProjectEntity> matchingProjects = new List<ProjectEntity>();
        try
        {
            projects = await _projectRepository.GetAsync();
            if (projects == null)
            {
                return Result.NotFound("There are no projects registered.");
            }
            else
            {
                foreach (var entity in projects)
                {
                    if (entity.EndDate.Equals(endDate))
                    {
                        matchingProjects.Add(entity);
                    }
                }
                
                if (matchingProjects.Count == 0)
                {
                    return Result.NotFound("No projects with this end date exists.");
                }
                else
                {
                    return Result<List<ProjectEntity>>.Ok(matchingProjects);
                }
            }
        }
        catch
        {
            return Result.Error("Something went wrong");
        }
    }

    public async Task<IResult> UpdateProject(int id, ProjectEntity updatedProject)
    {
        await _projectRepository.BeginTransactionAsync();
        try
        {
            ProjectEntity existingProject = await _projectRepository.GetAsync(p => p.ProjectId == id);
            
            if (existingProject == null)
            {
                return Result.NotFound("Could not find the project in the database.");
            }
            else
            {
                //In this part I had some suggestions from Claude AI on how to update the team members correctly. 
                var teamMembers = updatedProject.TeamMembers;

                existingProject.ProjectName = updatedProject.ProjectName;
                existingProject.StartDate = updatedProject.StartDate;
                existingProject.EndDate = updatedProject.EndDate;
                existingProject.Description = updatedProject.Description;
                existingProject.Budget = updatedProject.Budget;
                existingProject.ClientId = updatedProject.ClientId;
                existingProject.StatusId = updatedProject.StatusId;
                existingProject.ProjectImagePath = updatedProject.ProjectImagePath;
                existingProject.TeamMembers.Clear();

                foreach (var teamMember in updatedProject.TeamMembers)
                {
                    existingProject.TeamMembers.Add(new ProjectEmployeeEntity
                    {
                        ProjectId = existingProject.ProjectId,
                        EmployeeId = teamMember.EmployeeId,
                    });
                }
            }

            await _projectRepository.SaveAsync();
            await _projectRepository.CommitTransactionAsync();
            return Result<ProjectEntity>.Ok(existingProject);
        }
        catch
        {
            return Result.Error("Something went wrong");
        }
    }

    public async Task<IResult> DeleteProject(int id)
    {
        await _projectRepository.BeginTransactionAsync();
        try
        {
            bool deleted = await _projectRepository.DeleteAsync(p => p.ProjectId == id);
            if (deleted)
            {
                await _projectRepository.SaveAsync();
                await _projectRepository.CommitTransactionAsync();
                return Result.Ok();
            }

            return Result.Error("Something went wrong.");
        }
        catch
        {
            await _projectRepository.RollbackTransactionAsync();
            return Result.Error("Something went wrong.");
        }
    }
}
