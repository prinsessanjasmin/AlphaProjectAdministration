using Business.Interfaces;
using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp_MVC.Controllers
{
    //[Route("teammembers")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly DataContext _dataContext;

        public EmployeeController(DataContext dataContext, IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            _dataContext = dataContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> SearchMembers(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new List<object>());
            }

            var result = await _employeeService.GetEmployeesBySearchTerm(term);
            if (result.Success)
            {
                var mappedResults = result as IResult<IEnumerable<EmployeeEntity>>;
                var employeeList = mappedResults?.Data?.Select(e => new
                {
                    Id = e.Id.ToString(),
                    MemberFullName = $"{e.FirstName} {e.LastName}",
                    ProfileImage = e.ProfileImagePath?.Replace("~", "") ?? "/ProjectImages/Icons/Avatar.svg"
                }).ToList();

                Console.WriteLine($"Found {employeeList.Count} employees for term: {term}");

                return Json(new { data = employeeList } );
            }

            return Json(new List<object>());
        }
    }
}
