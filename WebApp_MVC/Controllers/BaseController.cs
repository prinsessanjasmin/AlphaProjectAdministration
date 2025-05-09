﻿using Microsoft.AspNetCore.Mvc;

namespace WebApp_MVC.Controllers;

//I had ChatGPT 4o draw this up for me as a way of not having to handle the JSON and AJAX
//validation stuff in every controller that handles a form. Claude AI has also been involved when stuff didn't work as intended. 
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

    protected IActionResult AjaxResult(bool success, string redirectUrl = null, string message = null)
    {
        Response.ContentType = "application/json";
        // Use Json() instead of Ok() to ensure proper content type handling
        return Json(new
        {
            success,
            redirectUrl,
            message
        });
    }

    protected IActionResult ReturnBasedOnRequest(object model, string viewName = null)
    {
        if (IsAjaxRequest())
        {
            if (!ModelState.IsValid)
            {
                return JsonValidationErrors();
            }

            // If model is valid but we still need to return the view (like for AJAX form load)
            if (viewName != null && viewName.StartsWith("_"))
            {
                return PartialView(viewName, model);
            }
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

