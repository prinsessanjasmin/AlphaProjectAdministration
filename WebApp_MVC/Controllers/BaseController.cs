using Microsoft.AspNetCore.Mvc;

namespace WebApp_MVC.Controllers;

//I had ChatGPT 4o draw this up for me as a way of not having to handle the JSON and AJAX
//validation stuff in every controller that handles a form. 
public class BaseController : Controller
{
    protected bool IsAjaxRequest()
    {
        return Request?.Headers["X-Requested-With"].ToString().Equals("XMLHttpRequest", StringComparison.OrdinalIgnoreCase) == true;
    }

    protected BadRequestObjectResult JsonValidationErrors()
    {
        Response.ContentType = "application/json";

        var errors = ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        return BadRequest(new
        {
            success = false,
            errors
        });
    }

    protected IActionResult AjaxResult(bool success, object data = null, string redirectUrl = null, string message = null)
    {
        Response.ContentType = "application/json";

        return Ok(new
        {
            success,
            data,
            redirectUrl,
            message
        });
    }

    protected IActionResult ReturnBasedOnRequest(object model, string viewName = null)
    {
        if (IsAjaxRequest())
        {
            return JsonValidationErrors();
        }

        return viewName != null ? View(viewName, model) : View(model);
    }

    protected IActionResult ReturnViewOrPartial(string viewName, object model)
    {
        if (IsAjaxRequest())
        {
            return PartialView(viewName, model);
        }

        return View(viewName, model);
    }
}

