using Business.Models;
using Data.Entities;

namespace Business.Interfaces;

public interface IUserService
{
    Task<IResult> RegisterUser(AppUserDto dto);
    Task<IResult> UpdateUserProfile(UpdateUserDto dto);
    Task<IResult> CreateEmployee(EmployeeDto dto);
    Task<IResult<IEnumerable<ApplicationUser>>> GetAllUsers();
    Task<IResult> GetUserById(string id);
    Task<IResult> GetUserByEmail(string email);
    Task<IResult> GetUsersBySearchTerm(string term);
    Task<IResult> UpdateUser(string id, ApplicationUser updatedUser); 
    Task<IResult> DeleteUser(string id);
}
