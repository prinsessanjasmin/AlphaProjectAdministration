using Microsoft.AspNetCore.Mvc;

namespace WebApp_MVC.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Project");
    }
}
