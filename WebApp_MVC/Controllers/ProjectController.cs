using Business.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using WebApp_MVC.Models;

namespace WebApp_MVC.Controllers
{
    public class ProjectController(AddProjectViewModel addProjectViewModel) : Controller
    {
        private readonly AddProjectViewModel _addProjectViewModel = addProjectViewModel;

        public async Task<IActionResult> Index()
        {
            await _addProjectViewModel.PopulateMemberOptionsAsync(); 
            return View(_addProjectViewModel);
        }

        public async Task<IActionResult> AddProject(ProjectFormModel formData)
        {
            if (!ModelState.IsValid)
            {
                await _addProjectViewModel.PopulateMemberOptionsAsync();
                _addProjectViewModel.FormData = formData; 
                return View(formData);
            }
            // glöm ej att här skicka infon till databasen innan den töms 

            _addProjectViewModel.ClearFormData();
            return View(_addProjectViewModel); 
                //Eller redirecta 
        }

        public IActionResult EditProject()
        {
            return View();
        }

        public IActionResult DeleteProject()
        {
            return View();
        }
    }
}
