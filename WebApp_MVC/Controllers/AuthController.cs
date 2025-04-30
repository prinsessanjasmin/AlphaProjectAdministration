using Business.Models;
using Business.Interfaces;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApp_MVC.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using WebApp_MVC.Hubs;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Business.Helpers;

namespace WebApp_MVC.Controllers;

public class AuthController(IUserService userService, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IHubContext<NotificationHub> notificationHub, INotificationService notificationService) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;

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
            Console.WriteLine(ModelState.ErrorCount);
            Console.WriteLine(ModelState.Values);
            return View(form);
        }

        var result = await _signInManager.PasswordSignInAsync(form.Email, form.Password, form.RememberMe, false);
        if (!result.Succeeded)
        {
            Console.WriteLine($"Login failed for {form.Email}. Result: {result.ToString()}");

            var appUser = await _userManager.FindByEmailAsync(form.Email);
            if (appUser == null)
            {
                Console.WriteLine("User not found");
            }
            else
            {
                Console.WriteLine($"User found: {appUser.Id}");
                // Check if password is correct (without revealing actual password)
                var isPasswordValid = await _userManager.CheckPasswordAsync(appUser, form.Password);
                Console.WriteLine($"Password valid: {isPasswordValid}");
            }
            return View(form);
        }

        var user = await _userManager.FindByEmailAsync(form.Email);
            if (user != null)
            {
                if (!User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                {
                    await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.NameIdentifier, user.Id));
                }

                await AddClaimByEmailAsync(user, "DisplayName", $"{user.FirstName} {user.LastName}");
                var notification = new NotificationEntity
                {
                    Message = $"{user.FirstName} {user.LastName} signed in.",
                    NotificationTypeId = 1,
                    TargetGroupId = 2
                };
                await _notificationService.AddNotificationAsync(notification);
                var notifications = await _notificationService.GetAllAsync(user.Id);
                var newNotification = notifications.OrderByDescending(x => x.Created).FirstOrDefault();

                if (newNotification != null)
                {
                    await _notificationHub.Clients.All.SendAsync("ReceiveNotification", newNotification);
                }
            }

            user.IsProfileComplete = UserProfileHelper.IsProfileComplete(user);
            await _userManager.UpdateAsync(user);

            if (!user.IsProfileComplete)
            {
                return RedirectToAction("UpdateUserProfile", new { Id = user.Id });
            }

            if (!Url.IsLocalUrl(returnUrl))
            {
                returnUrl = Url.Action("Index", "Project") ?? string.Empty;
            }

            return Redirect(returnUrl);
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
            ModelState.AddModelError("", "Complete the form.");
            return View(model);
        }

        AppUserDto dto = model;
        var result = await _userService.RegisterUser(dto); 
       
        if (result.Success)
        {
            var userResult = result as IResult<ApplicationUser>;
            if (userResult.Success)
            {
                var userId = userResult.Data.Id;
                var userName = userResult.Data.UserName;

                await _signInManager.SignInAsync(userResult.Data, isPersistent: false);
                return RedirectToAction("UpdateUserProfile", "Auth", new { id = userId });
            }
            else
            {
                ViewBag.ErrorMessage("Couldn't find user id.");
                return View(model);
            }
        }
        else
        {
            ViewBag.ErrorMessage(result.ErrorMessage);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> UpdateUserProfile(string id)
    {
        var result = await _userService.GetUserById(id);

        if (!result.Success)
        {
            ViewBag.ErrorMessage = "No team member found";
            return View(new UpdateUserViewModel()); // prevent null model
        }

        var user = ((Result<ApplicationUser>)result).Data;
        var viewModel = new UpdateUserViewModel(user);
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUserProfile(UpdateUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .Select(x => new {
                Key = x.Key,
                Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
            })
            .ToList();

            // Output errors to debug window or log
            foreach (var error in errors)
            {
                System.Diagnostics.Debug.WriteLine($"Property: {error.Key}, Errors: {string.Join(", ", error.Errors)}");
            }

            return View(model);

        }

        var dto = (UpdateUserDto)model;
        var result = await _userService.UpdateUserProfile(dto);

        if (result.Success)
            
            return RedirectToAction("Index", "Project");

        ViewBag.ErrorMessage = "error";
        return View(model);
    }

    [HttpPost]
    public new async Task<IActionResult> SignOut()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("SignIn", "Auth");
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
            var user = await _userManager.FindByLoginAsync(
                externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey
                );

            user.IsProfileComplete = UserProfileHelper.IsProfileComplete(user);
            await _userManager.UpdateAsync(user);

            if (user != null && !user.IsProfileComplete)
            {
                return RedirectToAction("UpdateUserProfile", new {Id = user.Id});
            }

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
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.NameIdentifier, user.Id));
                await _signInManager.SignInAsync(user, isPersistent: false);

                user.IsProfileComplete = UserProfileHelper.IsProfileComplete(user);
                await _userManager.UpdateAsync(user);

                if (user != null && !user.IsProfileComplete)
                {
                    return RedirectToAction("UpdateUserProfile", new { Id = user.Id });
                }

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
