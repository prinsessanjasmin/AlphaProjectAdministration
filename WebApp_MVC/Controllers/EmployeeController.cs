using Microsoft.AspNetCore.Mvc;

namespace WebApp_MVC.Controllers
{
    //[Route("teammembers")]
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
