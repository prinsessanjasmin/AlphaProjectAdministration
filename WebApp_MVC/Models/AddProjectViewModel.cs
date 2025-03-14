using Business.Models;
using Business.Services;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp_MVC.Models;

public class AddProjectViewModel
{
    private readonly IEmployeeService _employeeService;


    public ProjectFormModel FormData = new();
    public List<SelectListItem> MemberOptions = [];

    public AddProjectViewModel(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    //public async Task PopulateMemberOptionsAsync()
    //{
    //    var members = await _employeeService.GetAllAsync();
    //    MemberOptions = [.. members.Select(x => SelectListItem
    //    {
    //        Value = x.Id.ToString(), 
    //        Text = x.Name
    //    }).ToList();
    //}

    public void ClearFormData()
    {
        FormData = new();
    }
}
