using Business.Interfaces;
using Business.Models;
using Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApp_MVC.Controllers;

public class SearchController(ISearchService searchService) : Controller
{
    private readonly ISearchService _searchService = searchService;

    //Great deal of help here from ChatGPT 4o AND Claude AI 
    public async Task<JsonResult> Search(string query)
    {
        try
        {
            if (string.IsNullOrEmpty(query))
            {
                return Json(new List<object>());
            }

            Console.WriteLine($"Searching for: {query}");

            var searchResult = await _searchService.SearchAsync(query);

            Console.WriteLine($"Search results - Employees: {searchResult.Employees.Count}, Projects: {searchResult.Projects.Count}, Clients: {searchResult.Clients.Count}");

            var formattedResults = new List<object>();

            formattedResults.AddRange(searchResult.Employees.Select(e => new {
                Id = e.Id,
                DisplayText = e.DisplayProperty,
                EntityType = "Employee",
                DetailsUrl = e.DetailsUrl
            }));

            formattedResults.AddRange(searchResult.Projects.Select(p => new {
                Id = p.Id,
                DisplayText = p.DisplayProperty,
                EntityType = "Project",
                DetailsUrl = p.DetailsUrl
            }));

            formattedResults.AddRange(searchResult.Clients.Select(c => new {
                Id = c.Id,
                DisplayText = c.DisplayProperty,
                EntityType = "Client",
                DetailsUrl = c.DetailsUrl
            }));

            Console.WriteLine($"Formatted results: {formattedResults.Count}");

            return Json(new { data = formattedResults });
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Search error: {ex.Message}");
            return Json(new { error = "An error occurred during search", message = ex.Message });
        }
    }
}
