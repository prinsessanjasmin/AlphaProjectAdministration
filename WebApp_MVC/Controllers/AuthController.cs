using Business.Models;
using Business.Interfaces;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApp_MVC.Models;
using System.Security.Claims;

namespace WebApp_MVC.Controllers;

public class AuthController(IUserService userService, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [Route("signin")]
    public IActionResult SignIn(string returnUrl = "/")
    {
        ViewBag.ErrorMessage = "";
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignIn(LoginFormViewModel form, string returnUrl = "/")
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
            //var user = await _userManager.FindByEmailAsync(form.Email);
            //if (user != null)
            //{
            //    await AddClaimByEmailAsync(user, "DisplayName", $"{user.FirstName} {user.LastName}");
            //}
            return RedirectToAction("Index", "Project"); 
        }
            
        ViewBag.ErrorMessage = "Incorrect email or password";
        return View(form);
    }

    public async Task AddClaimByEmailAsync(ApplicationUser user, string typeName, string value)
    {
        if (user != null)
        {
            var claims = await _userManager.GetClaimsAsync(user);

            if (!claims.Any(x => x.Type == typeName))
            {
                await _userManager.AddClaimAsync(user, new Claim(typeName, value));
            }
        }
    }
    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpViewModel model)
    {
        if (!ModelState.IsValid) 
        {
            return View(model);
        }

        var result = await _userService.CreateUser(model); 
        
        switch (result.StatusCode)
        {
            case 201:
                return RedirectToAction("SignIn", "Auth");

            case 400:
                ViewBag.ErrorMessage = "Error."; 
                return View(model);

            case 404:
                ViewBag.ErrorMessage = "Error.";
                return View(model);

            case 500:
                ViewBag.ErrorMessage = "Error.";
                return View(model);

            default:
                ViewBag.ErrorMessage = "Error.";
                return View(model);
        }
    }

    public new async Task<IActionResult> SignOut()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult ExternalSignIn(string provider, string returnUrl = null!)
    {
        if (string.IsNullOrEmpty(provider))
        {
            ModelState.AddModelError("", "Invalid provider");
            return View("SignIn");
        }

        var redirectUrl = Url.Action("ExternalSignInCallback", "Auth", new { returnUrl })!;
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        return Challenge(properties, provider);
    }

    public async Task<IActionResult> ExternalSignInCallback(string returnUrl = null!, string remoteError = null!)
    {
        returnUrl ??= Url.Content("~/");
        if (!string.IsNullOrEmpty(remoteError))
        {
            ModelState.AddModelError("", $"Errror from external provider: {remoteError}");
            return View("SignIn"); 
        }

        var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
        if (externalLoginInfo == null)
        {
            return RedirectToAction("SignIn");
        }

        var signInResult = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (signInResult.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }
        else
        {
            string email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email)!;
            string userName = $"ext_{externalLoginInfo.LoginProvider.ToLower()}_{email}";
            string firstName = string.Empty;
            string lastName = string.Empty;

            try
            {
                firstName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.GivenName)!;
                lastName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Surname)!;
            }
            catch {}

            var user = new ApplicationUser { UserName = userName, Email = email, FirstName = firstName, LastName = lastName };
            var identityResult = await _userManager.CreateAsync(user);
            if (identityResult.Succeeded) 
            {
                await _userManager.AddLoginAsync(user, externalLoginInfo); 
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            foreach(var error in identityResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View("SignIn");
        }
    }
}
