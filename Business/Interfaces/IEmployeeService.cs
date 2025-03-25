using Business.Models;
using Data.Entities;

namespace Business.Interfaces;

public interface IEmployeeService
{
    Task<IResult> CreateEmployee(MemberDto form);
    Task<IResult> GetAllEmployees();
    Task<IResult> GetEmployeeById(int id);
    Task<IResult> GetEmployeeByEmail(string email);
    Task<IResult> UpdateEmployee(int id, EmployeeEntity updatedEmployee);
    Task<IResult> DeleteEmployee(int id);
}
