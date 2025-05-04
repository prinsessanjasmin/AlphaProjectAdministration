using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebApp_MVC.Models;

namespace WebApp_MVC.Controllers;

public class CookiesController : Controller
{
    [HttpPost]
    public IActionResult SetCookies([FromBody] CookieConsent consent)
    {
        
        if (consent == null)
            return BadRequest(); 

        if (consent.Functional)
        {
            Response.Cookies.Append("FunctionalCookies", "Non-Esstential", new CookieOptions
            {
                IsEssential = false,
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });
        }
        else
        {
            Response.Cookies.Delete("FunctionalCookie"); 
        }

        if (consent.Analytical)
        {
            Response.Cookies.Append("AnalyticalCookie", "Non-Esstential", new CookieOptions
            {
                IsEssential = false,
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });
        }
        else
        {
            Response.Cookies.Delete("AnalyticalCookie");
        }

        if (consent.Marketing)
        {
            Response.Cookies.Append("MarketingCookie", "Non-Esstential", new CookieOptions
            {
                IsEssential = false,
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });
        }
        else
        {
            Response.Cookies.Delete("MarketingCookie");
        }

        Response.Cookies.Append("cookieConsent", JsonSerializer.Serialize(consent), new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(90),
            SameSite = SameSiteMode.Lax,
            Path = "/"
        });

        return Ok();
    }
}
