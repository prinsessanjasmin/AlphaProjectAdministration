using Microsoft.AspNetCore.Mvc;

namespace WebApp_MVC.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
