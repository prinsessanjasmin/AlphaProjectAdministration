using Business.Interfaces;
using Business.Services;
using Microsoft.AspNetCore.Mvc;
using WebApp_MVC.Models;
using System.Threading.Tasks;
using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json;
using Business.Factories;
using Microsoft.AspNetCore.Hosting;

namespace WebApp_MVC.Controllers;

//[Route("clients")]
public class ClientController(IClientService clientService) : Controller
{
    private readonly IClientService _clientService = clientService;

    public async Task<IActionResult> Index()
    {
        var clients = await _clientService.GetAllClients();
        var model = new ClientViewModel(_clientService);

        if (clients.Success)
        {
            var clientResult = clients as Result<IEnumerable<ClientEntity>>;
            model.Clients = clientResult?.Data?.ToList() ?? [];
        }
        else
        {
            model.Clients = [];
        }
        var viewModel = new AddClientViewModel();

        ViewData["ClientFormViewModel"] = viewModel;
        return View(viewModel);
    }

    public async Task<IActionResult> AddClient()
    {
        if (ViewData["AddProjectViewModel"] is AddClientViewModel viewModel)
        {
            return PartialView("_AddClient", viewModel);
        }

        var fallbackModel = new AddClientViewModel();
        return PartialView("_AddClient", fallbackModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddClient(AddClientViewModel form)
    {
        if (!ModelState.IsValid)
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage)
                .ToList()
                );

                return BadRequest(new { success = false, errors });
            }
            else
            {
                return View("_AddClient", form);
            }
        }

        ClientDto clientDto = form; 
        var result = await _clientService.CreateClient(clientDto);
        if (result.Success)
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return Ok(new { success = true, message = "Client created successfully" });
            }
            else
            {
                return RedirectToAction("Index", "Client");
            }
           
        }
        else
        {
            ModelState.AddModelError("", "Something went wrong when creating the client");
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return BadRequest(new { success = false, message = "Failed to create client" });
            }
            else
            {
                return PartialView("_AddClient", form);
            }
        }
    }

    public async Task<IActionResult> EditClient(int id)
    {
        var result = await _clientService.GetClientById(id);

        if (result.Success)
        {
            var clientResult = result as Result<ClientEntity>;
            ClientEntity client = clientResult?.Data ?? new ClientEntity();

            var viewModel = new EditClientViewModel(client);

            return PartialView("_EditClient", viewModel);
        }
        else
        {
            ViewBag.ErrorMessage("No client found");
            return RedirectToAction("Index", "Client");
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditClient(EditClientViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage)
                .ToList()
                );

            return PartialView("_EditClient", model);
        }

        ClientDto clientDto = model;

        
        ClientEntity clientEntity = ClientFactory.Create(clientDto);

        var result = await _clientService.UpdateClient(model.Id, clientEntity);

        if (result.Success)
        {
            return RedirectToAction("Index", "Client");
        }
        else
        {
            ViewBag.ErrorMessage("Something went wrong.");
            return PartialView("_EditClient", model);
        }
    }

    public IActionResult ConfirmDelete(int id)
    {
        // Return just the ID to the partial view
        return PartialView("_DeleteClient", id);
    }

    [HttpPost]
    //[Route("delete")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var result = await _clientService.DeleteClient(id);

        if (result.Success)
        {
            return RedirectToAction("Index", "Client");
        }
        else
        {
            ViewBag.ErrorMessage("Something went wrong.");
            return RedirectToAction("Index", "Client");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var result = await _clientService.GetClientById(id);

        if (!result.Success)
        {
            return View("Index");
        }

        var clientResult = result as Result<ClientEntity>;
        ClientEntity client = clientResult?.Data ?? new();

        var viewModel = new ClientDetailsViewModel
        {
            Id = client.Id,
            ClientName = client.ClientName,
            Projects = client.Projects.ToList(),
        };

        return View(viewModel);
    }
}
