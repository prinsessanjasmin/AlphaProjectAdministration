using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<IResult> CreateUser(SignUpFormModel form)
    {
        if (!form.AcceptTerms)
        {
            return Result.BadRequest("You have to accept the terms and conditions to proceed."); 
        }

        try
        {
            ApplicationUser userEntity = UserFactory.Create(form);
            await _userRepository.CreateUserAsync(userEntity, form.Password, form.Role);
            return Result<ApplicationUser>.Created(userEntity);
        }
        catch
        {
            return Result.Error("Unable to create user.");
        }
    }

    public async Task<IResult> GetAllUsers()
    {
        try
        {
            IEnumerable<ApplicationUser> users = await _userRepository.GetAllUsersAsync();
            if (users == null)
            {
                return Result.NotFound("No users were found.");
            }
                
            return Result<IEnumerable<ApplicationUser>>.Ok(); 
        }
        catch
        {
            return Result.Error("Something went wrong."); 
        }
    }

    public async Task<IResult> GetUserByEmail(string email)
    {
       try
        {
            ApplicationUser? user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return Result.NotFound("The user couldn't be found. Are you sure you entered the correct email address?");
            }
            return Result<ApplicationUser>.Ok(user); 
        }
        catch
        {
            return Result.Error("Something went wrong.");
        }
    }

    public async Task<IResult> GetUserById(string id)
    {
        try
        {
            ApplicationUser? user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return Result.NotFound("The user couldn't be found.");
            }
            return Result<ApplicationUser>.Ok(user);
        }
        catch
        {
            return Result.Error("Something went wrong.");
        }
    }

    public async Task<IResult> UpdateUser(string id, ApplicationUser updatedUser) //Eller använd den andra modellen? 
    {
        try
        {
            IdentityResult user = await _userRepository.UpdateUserAsync(updatedUser);
            if (user == null)
                return Result.NotFound("Could not find the user in the database.");

            return Result<ApplicationUser>.Ok();
        }
        catch
        {
            return Result.Error("Something went wrong");
        }
    }

    public async Task<IResult> DeleteUser(string id)
    {
        try
        {
            ApplicationUser? user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return Result.NotFound("The user you want to delete couldn't be found.");
            }
            await _userRepository.DeleteUserAsync(user);
            
            return Result.Ok();
        }
        catch
        {
            return Result.Error("Something went wrong.");
        }
    }
}
