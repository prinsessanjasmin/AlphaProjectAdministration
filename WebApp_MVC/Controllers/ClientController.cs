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
using Microsoft.AspNetCore.Authorization;

namespace WebApp_MVC.Controllers;

[Authorize]
public class ClientController(IClientService clientService, INotificationService notificationService) : BaseController
{
    private readonly IClientService _clientService = clientService;
    private readonly INotificationService _notificationService = notificationService;


    [HttpGet]
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

        ViewData["AddClientViewModel"] = viewModel;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> AddClient()
    {
        if (ViewData["AddClientViewModel"] is AddClientViewModel viewModel)
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
            return ReturnBasedOnRequest(form, "_AddClient");
        }

        ClientDto clientDto = form;
        var result = await _clientService.CreateClient(clientDto);
        if (result.Success)
        {
            string notificationMessage = $"New client '{form.ClientName}' added.";
            var notification = NotificationFactory.Create(2, 3, notificationMessage, "");
            await _notificationService.AddNotificationAsync(notification);

            return AjaxResult(true, redirectUrl: Url.Action("Index", "Client"), message: "Client created.");
        }
        ModelState.AddModelError("", "Something went wrong when creating the client");
        return ReturnBasedOnRequest(form, "_AddClient");
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
            return ReturnBasedOnRequest(model, "_EditClient");
        }

        ClientDto clientDto = model;

        ClientEntity clientEntity = ClientFactory.Create(clientDto);

        var result = await _clientService.UpdateClient(model.Id, clientEntity);

        if (result.Success)
        {
            return AjaxResult(true, redirectUrl: Url.Action("Index", "Client"), message: "Client edited.");
        }
        else
        {
            ViewBag.ErrorMessage("Something went wrong.");
            return ReturnBasedOnRequest(model, "_EditClient");
        }
    }

    public IActionResult ConfirmDelete(int id)
    {
        // Return just the ID to the partial view
        return PartialView("_DeleteClient", id);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var result = await _clientService.DeleteClient(id);

        if (result.Success)
        {
            return RedirectToAction("Index", "Client");
        }

        ViewBag.ErrorMessage("Something went wrong.");
        return RedirectToAction("Index", "Client");
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
            Projects = [.. client.Projects],
        };

        return View(viewModel);
    }
}
