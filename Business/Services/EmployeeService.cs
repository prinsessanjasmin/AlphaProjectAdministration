using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;

namespace Business.Services;

public class EmployeeService(IEmployeeRepository employeeRepository, IAddressService addressService) : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;
    private readonly IAddressService _addressService = addressService; 


    public async Task<IResult> CreateEmployee(MemberDto form)
    {
        await _employeeRepository.BeginTransactionAsync();
        try
        {
            if (form == null) 
            {
                return Result.BadRequest("You need to fill out the form.");
            }

            if (await _employeeRepository.AlreadyExistsAsync(x => x.Email == form.Email))
            {
                return Result.AlreadyExists("The email adress you are trying to register already exists."); 
            }

            var result = await _addressService.CreateAddress(form);

            if (!result.Success || result.Data == null)
                return Result.Error("Something went wrong when handling the address.");
               
            var entity = EmployeeFactory.Create(form, result.Data.Id);
            EmployeeEntity employee = await _employeeRepository.CreateAsync(entity);
            await _employeeRepository.SaveAsync();
            await _employeeRepository.CommitTransactionAsync();
            return Result<EmployeeEntity>.Created(employee);
        }
        catch
        {
            await _employeeRepository.RollbackTransactionAsync();
            return Result.Error("Something went wrong."); 
        }
    }

    public async Task<IResult> GetAllEmployees()
    {
        try
        {
            var employees = await _employeeRepository.GetAsync();
            if (employees != null)
            {
                return Result<IEnumerable<EmployeeEntity>>.Ok(employees);
            }
            return Result.NotFound("There are no team members.");
        }
        catch
        {
            return Result.Error("Something went wrong");
        }
    }

    public async Task<IResult> GetEmployeeByEmail(string email)
    {
        try
        {
            EmployeeEntity employee = await _employeeRepository.GetAsync(e => e.Email == email);
            if (employee == null)
            {
                return Result.NotFound("An employee with that email address wasn't found.");
            }

            return Result<EmployeeEntity>.Ok(employee);
        }
        catch
        {
            return Result.Error("Something went wrong");
        }
    }

    public async Task<IResult> GetEmployeeById(int id)
    {
        try
        {
            EmployeeEntity employee = await _employeeRepository.GetAsync(e => e.Id == id);
            if (employee == null)
            {
                return Result.NotFound("An employee with that id wasn't found.");
            }

            return Result<EmployeeEntity>.Ok(employee);
        }
        catch
        {
            return Result.Error("Something went wrong");
        }
    }

    public async Task<IResult> UpdateEmployee(int id, EmployeeEntity updatedEmployee)
    {
        await _employeeRepository.BeginTransactionAsync();
        try
        {
            EmployeeEntity employee = await _employeeRepository.UpdateAsync(e => e.Id == id, updatedEmployee);
            if (employee == null) 
                return Result.NotFound("Could not find the employee in the database.");

            await _employeeRepository.SaveAsync();
            await _employeeRepository.CommitTransactionAsync(); 
            return Result<EmployeeEntity>.Ok(employee);
        }
        catch
        {
            return Result.Error("Something went wrong");
        }
    }

    public async Task<IResult> DeleteEmployee(int id)
    {
        await _employeeRepository.BeginTransactionAsync(); 
        try
        {
            bool deleted = await _employeeRepository.DeleteAsync(e => e.Id == id);
            if (deleted)
            {
                await _employeeRepository.SaveAsync();
                await _employeeRepository.CommitTransactionAsync();
                return Result.Ok();
            }
            return Result.Error("Something went wrong.");
        }
        catch
        {
            await _employeeRepository.RollbackTransactionAsync();
            return Result.Error("Something went wrong."); 
        }
    }
}
