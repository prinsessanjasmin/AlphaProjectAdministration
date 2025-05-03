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
using Microsoft.AspNetCore.Authorization;

namespace WebApp_MVC.Controllers;

public class AuthController(IUserService userService, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IHubContext<NotificationHub> notificationHub, INotificationService notificationService, RoleManager<IdentityRole> roleManager) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
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

        var user = await _userManager.FindByEmailAsync(form.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(form);
        }

        // Special handling for migrating password hashes
        // First try with normal password validation
        var result = await _signInManager.PasswordSignInAsync(form.Email, form.Password, form.RememberMe, false);
        ViewBag.Debug += $" | Initial sign-in: {result.Succeeded}";

        if (!result.Succeeded)
        {
            // If normal sign-in fails, try manually checking if it's a hash mismatch
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, form.Password);
            ViewBag.Debug += $" | Password valid: {isPasswordValid}";

            if (!isPasswordValid)
            {
                // Genuinely wrong password
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(form);
            }

            // Password is correct but hash validation failed - rehash password
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResetResult = await _userManager.ResetPasswordAsync(user, token, form.Password);
            ViewBag.Debug += $" | Password reset: {passwordResetResult.Succeeded}";

            if (passwordResetResult.Succeeded)
            {
                // Try sign-in again with updated hash
                result = await _signInManager.PasswordSignInAsync(user, form.Password, form.RememberMe, false);
                ViewBag.Debug += $" | Second sign-in: {result.Succeeded}";

                if (!result.Succeeded)
                {
                    if (result.IsLockedOut)
                        ViewBag.Debug += " | Account locked";
                    if (result.IsNotAllowed)
                        ViewBag.Debug += " | Sign-in not allowed";
                    if (result.RequiresTwoFactor)
                        ViewBag.Debug += " | 2FA required";

                    ModelState.AddModelError(string.Empty, "Login failed after password reset.");
                    return View(form);
                }
            }
            else
            {
                string errors = string.Join(", ", passwordResetResult.Errors.Select(e => e.Description));
                ViewBag.Debug += $" | Reset errors: {errors}";
                ModelState.AddModelError(string.Empty, "Password migration failed.");
                return View(form);
            }
        }


        user = await _userManager.FindByEmailAsync(form.Email);
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

    [HttpGet]
    public IActionResult AdminSignIn(string returnUrl = "/")
    {
        ViewBag.ErrorMessage = "";
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AdminSignIn(AdminLoginFormViewModel form, string returnUrl = "/")
    {
        if (!ModelState.IsValid)
        {
            Console.WriteLine(ModelState.ErrorCount);
            Console.WriteLine(ModelState.Values);
            return View(form);
        }

        var user = await _userManager.FindByEmailAsync(form.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(form);
        }

        // Special handling for migrating password hashes
        // First try with normal password validation
        var result = await _signInManager.PasswordSignInAsync(form.Email, form.Password, form.RememberMe, false);

        if (!result.Succeeded)
        {
            // If normal sign-in fails, try manually checking if it's a hash mismatch
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, form.Password);

            if (!isPasswordValid)
            {
                // Genuinely wrong password
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(form);
            }

            // Password is correct but hash validation failed - rehash password
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResetResult = await _userManager.ResetPasswordAsync(user, token, form.Password);

            if (passwordResetResult.Succeeded)
            {
                // Try sign-in again with updated hash
                result = await _signInManager.PasswordSignInAsync(user, form.Password, form.RememberMe, false);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Login failed after password migration.");
                    return View(form);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Password migration failed.");
                return View(form);
            }
        }


        user = await _userManager.FindByEmailAsync(form.Email);
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

        if (!model.AcceptTerms)
        {
            ModelState.AddModelError("AcceptTerms", "You must accept the terms and conditions to register.");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        string role = string.IsNullOrEmpty(model.Role) ? "User" : model.Role;

        // Add user to role
        await _userManager.AddToRoleAsync(user, role);

        await _signInManager.SignInAsync(user, isPersistent: false);

        // Redirect to profile completion

        
        return RedirectToAction("UpdateUserProfile", new { Id = user.Id });
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
            .Select(x => new
            {
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
                return RedirectToAction("UpdateUserProfile", new { Id = user.Id });
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
            catch { }

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

            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View("SignIn");
        }
    }

    [AllowAnonymous]
    public IActionResult Denied()
    {
        return View();
    }

}
