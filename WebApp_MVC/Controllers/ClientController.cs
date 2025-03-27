using Microsoft.AspNetCore.Mvc;

namespace WebApp_MVC.Controllers;

//[Route("clients")]
public class ClientController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
