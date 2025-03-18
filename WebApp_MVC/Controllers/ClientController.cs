using Microsoft.AspNetCore.Mvc;

namespace WebApp_MVC.Controllers
{
    public class ClientController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
