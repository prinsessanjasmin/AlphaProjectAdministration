using Business.Models;
using Data.Entities;
using System.Reflection.Metadata.Ecma335;

namespace Business.Factories
{
    public static class ProjectFactory
    {
        public static ProjectEntity Create (ProjectFormModel form)
        {
            var projectEntity = new ProjectEntity
            {
                ProjectName = form.ProjectName,
                Description = form.Description,
                StartDate = form.StartDate,
                EndDate = form.EndDate,
                Budget = form.Budget,
                ClientId = form.ClientId,
                ProjectImage = form.ProjectImage,
                TeamMembers = new List<ProjectEmployeeEntity>()
            };

            //Claude AI helped me generate this code 
            if (form.SelectedTeamMemberIds != null && form.SelectedTeamMemberIds.Any())
            {
                foreach (var employeeId in form.SelectedTeamMemberIds)
                {
                    projectEntity.TeamMembers.Add(new ProjectEmployeeEntity
                    {
                        EmployeeId = employeeId,
                    });
                }
            }
            return projectEntity;
        }

        public static ProjectFormModel Create(ProjectEntity entity) 
        {
            var projectFormModel = new ProjectFormModel
            {
                ProjectName = entity.ProjectName,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                ClientId = entity.ClientId,
                Budget = entity.Budget,
                Description = entity.Description,

                // Convert the ProjectEmployeeEntity collection to a list of employee IDs Claude AI
                SelectedTeamMemberIds = entity.TeamMembers?
                .Select(pe => pe.EmployeeId)
                .ToList() ?? new List<int>()
            };

           
            return projectFormModel; 
        }
    }
}
