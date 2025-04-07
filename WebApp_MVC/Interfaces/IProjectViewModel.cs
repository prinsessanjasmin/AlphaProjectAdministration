using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp_MVC.Interfaces;

public interface IProjectViewModel
{
        List<SelectListItem> MemberOptions { get; set; }
        List<SelectListItem> ClientOptions { get; set; }
}
