using Business.Interfaces;
using Business.Services;
using Microsoft.AspNetCore.Mvc;
using WebApp_MVC.Models;
using System.Threading.Tasks;
using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace WebApp_MVC.Controllers;

//[Route("clients")]
public class ClientController(IClientService clientService) : Controller
{
    private readonly IClientService _clientService = clientService;

    public async Task<IActionResult> Index()
    {
        var viewModel = new ClientViewModel(_clientService);
        ViewData["ClientFormViewModel"] = viewModel;
        return View(viewModel);
    }

    public async Task<IActionResult> AddClient()
    {
        if (ViewData["AddProjectViewModel"] is ClientFormViewModel viewModel)
        {
            return PartialView("_AddClient", viewModel);
        }

        var fallbackModel = new AddProjectViewModel();
        return PartialView("_AddClient", fallbackModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddClient(ClientFormViewModel form)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_AddClient", form);
        }

        ClientDto clientDto = form; 
        var result = await _clientService.CreateClient(clientDto);
        if (result.Success)
        {
            return RedirectToAction("Index", "Client");
        }
        else
        {
            ViewBag.ErrorMessage("Something went wrong.");
            return PartialView("_AddClient", form);
        }
    }

}
