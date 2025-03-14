using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;

namespace Business.Services;

public class EmployeeService(IEmployeeRepository employeeRepository) : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;

    public async Task<IResult> CreateEmployee(MemberFormModel form)
    {
        await _employeeRepository.BeginTransactionAsync();
        try
        {
            if (form == null) 
            {
                return Result.BadRequest("You need to fillt out the form.");
            }
            if (await _employeeRepository.AlreadyExistsAsync(x => x.Email == form.Email))
            {
                return Result.AlreadyExists("The email adress you are trying to register already exists."); 
            }

            var entity = EmployeeFactory.Create(form);
            EmployeeEntity employee = await _employeeRepository.CreateAsync(entity);
            await _employeeRepository.CommitTransactionAsync();
            return Result<EmployeeEntity>.Created(employee); 
            
        }
        catch
        {
            await _employeeRepository.RollbackTransactionAsync();
            return Result.Error("Something went wrong."); 
        }
    }

    public async Task<IResult> DeleteEmployee(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> GetAllEmployees()
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> GetEmployeeByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> GetEmployeeById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> UpdateEmployee(int id, EmployeeEntity updatedEmployee)
    {
        throw new NotImplementedException();
    }
}
