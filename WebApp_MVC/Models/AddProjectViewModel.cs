using Business.Models;
using Business.Services;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp_MVC.Models;

public class AddProjectViewModel(IEmployeeService employeeService)
{
    private readonly IEmployeeService _employeeService = employeeService;


    public ProjectFormModel FormData = new();
    public List<SelectListItem> MemberOptions = [];

    public async Task PopulateMemberOptionsAsync()
    {
        await _employeeService.GetAllEmployees();
        //MemberOptions = [.. employees.Select(x => SelectListItem
        //{
        //    Value = x.EmployeeId.ToString(), 
        //    Text = x.FirstName
        //}).ToList();
    }

    public void ClearFormData()
    {
        FormData = new();
    }
}
