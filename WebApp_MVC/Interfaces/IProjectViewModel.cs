using Business.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp_MVC.Interfaces;

public interface IProjectViewModel
{
    List<SelectListItem> StatusOptions { get; set; }
    List<SelectListItem> ClientOptions { get; set; }
    List<TeamMemberDto> AvailableTeamMembers { get; set; }

    List<TeamMemberDto> PreselectedTeamMembers { get; set; }
    string SelectedTeamMemberIds { get; set; }
}
