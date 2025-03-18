using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business.Models;

namespace WebApp_MVC.Controllers;

[Authorize]
public class HomeController : Controller
{

    public IActionResult Index()
    {
        var formData = new SignUpFormModel(); 
        return View(formData);
    }

    [HttpPost]
    public IActionResult SignUp(SignUpFormModel formData)
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
