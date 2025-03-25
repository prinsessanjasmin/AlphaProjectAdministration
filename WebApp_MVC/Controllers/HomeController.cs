using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business.Models;
using WebApp_MVC.Models;

namespace WebApp_MVC.Controllers;

[Authorize]
public class HomeController : Controller
{

    public IActionResult Index()
    {
        var formData = new AppUserDto(); 
        return View(formData);
    }

    [HttpPost]
    public IActionResult SignUp(AppUserViewModel formData)
    {
        if (!ModelState.IsValid)
        {
            return View(formData);
        }
        return View();
    }

    [HttpPost]
    public IActionResult SignIn()
    {
        return View();
    }

    public IActionResult LogOut()
    {
        return RedirectToAction("Index", "HomeController"); 
    }


}
