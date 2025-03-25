using Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using WebApp_MVC.Models;

namespace WebApp_MVC.Controllers;

public class ProjectController(ProjectViewModel projectViewModel) : Controller
{
    private readonly ProjectViewModel _projectViewModel = projectViewModel;

    public async Task<IActionResult> Index()
    {
        
        await _projectViewModel.GetProjects(); // Ensure data is loaded before rendering the view
        return View(_projectViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddProject(ProjectViewModel formData)
    {
        if (!ModelState.IsValid)
        {
            await _projectViewModel.PopulateMemberOptionsAsync();
            await _projectViewModel.PopulateClientOptionsAsync();
            _projectViewModel.FormData = formData; 
            return View(formData);
        }
        // glöm ej att här skicka infon till databasen innan den töms 

        _projectViewModel.ClearFormData();
        return View(_projectViewModel); 
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
