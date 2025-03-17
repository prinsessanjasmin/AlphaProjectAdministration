using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;

namespace Business.Services;

public class EmployeeService(IEmployeeRepository employeeRepository, IAddressService addressService) : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;
    private readonly IAddressService _addressService = addressService; 


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

            //(Result, AddressEntity addressEntity) = await _addressService.CreateAddress(form); 

            var entity = EmployeeFactory.Create(form, addressEntity.Id);
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
