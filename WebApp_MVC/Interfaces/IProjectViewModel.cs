using Business.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp_MVC.Interfaces;

public interface IProjectViewModel
{
    List<SelectListItem> ClientOptions { get; set; }
    List<TeamMemberDto> AvailableTeamMembers { get; set; }

    List<TeamMemberDto> PreselectedTeamMembers { get; set; }
    string SelectedTeamMemberIds { get; set; }
}
