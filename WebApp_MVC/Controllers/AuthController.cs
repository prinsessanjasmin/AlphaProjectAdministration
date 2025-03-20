using Business.Models;
using Business.Services;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApp_MVC.Controllers
{
    public class AuthController(UserService userService, SignInManager<ApplicationUser> signInManager) : Controller
    {
        private readonly UserService _userService = userService;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

        public IActionResult SignIn(string returnUrl = "/")
        {
            ViewBag.ErrorMessage = "";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginFormModel form, string returnUrl = "/")
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Incorrect email or password"; ;
                ViewBag.ReturnUrl = returnUrl;
                return View(form);
            }

          
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(form.Email, form.Password, true, false);
            if (result.Succeeded)
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                
            }
                
            ViewBag.ErrorMessage = "Incorrect email or password";
            return View(form);
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpFormModel form)
        {
            if (!ModelState.IsValid) 
            {
                return View(form);
            }

            var result = await _userService.CreateUser(form); 

            
            
            //switch(result.StatusCode)
            //{
            //    case 201:
            //        return RedirectToAction("SignIn", "Auth");

            //    case 400: 
            //}

            return RedirectToAction("SignIn", "Auth");
        }

        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
