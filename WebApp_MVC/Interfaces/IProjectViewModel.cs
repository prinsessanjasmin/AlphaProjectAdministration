using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp_MVC.Interfaces;

public interface IProjectViewModel
{
        List<SelectListItem> ClientOptions { get; set; }
}
