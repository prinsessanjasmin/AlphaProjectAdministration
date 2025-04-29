using Business.Factories;
using Business.Helpers;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Business.Services;

public class UserService(IUserRepository userRepository, IAddressService addressService) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IAddressService _addressService = addressService;

    public async Task<IResult> RegisterUser(AppUserDto dto)
    {
        await _userRepository.BeginTransactionAsync();
        try
        {
            if (dto == null)
            {
                return Result.BadRequest("You need to fill out the form.");
            }

            if (await _userRepository.AlreadyExistsAsync(u => u.Email == dto.Email))
            {
                return Result.AlreadyExists("The email adress you are trying to register already exists.");
            }
           
            var userEntity = UserFactory.Create(dto);

            var user = await _userRepository.CreateUserAsync(userEntity, dto.Password, dto.Role);
            await _userRepository.SaveAsync();
            await _userRepository.CommitTransactionAsync();
            return Result<ApplicationUser>.Created(userEntity);
        }
        catch
        {
            await _userRepository.RollbackTransactionAsync();
            return Result.Error("Unable to create user.");
        }
    }

    

    public async Task<IResult> CreateEmployee(EmployeeDto dto)
    {
        await _userRepository.BeginTransactionAsync();
        try
        {
            if (dto == null)
            {
                return Result.BadRequest("You need to fill out the form.");
            }

            if (await _userRepository.AlreadyExistsAsync(u => u.Email == dto.Email))
            {
                return Result.AlreadyExists("The email adress you are trying to register already exists.");
            }

            var result = await _addressService.CreateAddress(dto);
            if (!result.Success || result.Data == null)
                return Result.Error("Something went wrong when handling the address.");

            var userEntity = UserFactory.Create(dto, result.Data.Id);

            var password = "Password123!";

            userEntity.IsProfileComplete = UserProfileHelper.IsProfileComplete(userEntity);

            var user = await _userRepository.CreateUserAsync(userEntity, password, dto.Role);
            await _userRepository.SaveAsync();
            await _userRepository.CommitTransactionAsync();
            return Result<ApplicationUser>.Created(userEntity);
        }
        catch
        {
            await _userRepository.RollbackTransactionAsync();
            return Result.Error("Unable to create user.");
        }
    }

    public async Task<IResult<IEnumerable<ApplicationUser>>> GetAllUsers()
    {
        try
        {
            IEnumerable<ApplicationUser> users = await _userRepository.GetAllUsersAsync();
            if (users != null && users.Any())
            {
                return Result<IEnumerable<ApplicationUser>>.Ok(users);
            }

            return Result<IEnumerable<ApplicationUser>>.NotFound("There are no team members.");
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ApplicationUser>>.Error("Something went wrong: " + ex.Message);
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

    public async Task<IResult> GetUsersBySearchTerm(string term)
    {
        if (string.IsNullOrEmpty(term))
        {
            return Result<List<ApplicationUser>>.Error("Search term cannot be empty.");
        }

        try
        {
            var users = await _userRepository.SearchByTermAsync(term);
            if (users == null || !users.Any())
            {
                return Result.NotFound("No users match your search.");
            }

            return Result<IEnumerable<ApplicationUser>>.Ok(users);
        }
        catch
        {
            return Result.Error("Something went wrong");
        }
    }

    public async Task<IResult> UpdateUser(string id, ApplicationUser updatedUser) 
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

    public async Task<IResult> UpdateUserProfile(UpdateUserDto dto)
    {
        await _userRepository.BeginTransactionAsync();
        try
        {
            if (dto == null)
            {
                return Result.BadRequest("You need to fill out the form.");
            }

            ApplicationUser? existingUser = await _userRepository.GetUserByIdAsync(dto.Id);
            if (existingUser == null)
            {
                return Result.NotFound("The user you want to delete couldn't be found.");
            }

            var addressResult = await _addressService.CreateAddress(dto);
            if (!addressResult.Success || addressResult.Data == null)
                return Result.Error("Something went wrong when handling the address.");

            if (existingUser.AddressId != addressResult.Data?.Id && addressResult.Data != null)
            {
                existingUser.AddressId = addressResult.Data.Id;
            }

            existingUser.PhoneNumber = dto.PhoneNumber ?? existingUser.PhoneNumber;
            existingUser.JobTitle = dto.JobTitle ?? existingUser.JobTitle;
            existingUser.ProfileImagePath = dto.ProfileImagePath ?? existingUser.ProfileImagePath;
            existingUser.DateOfBirth = dto.DateOfBirth;
            
           
            existingUser.IsProfileComplete = true; 

            var identityResult = await _userRepository.UpdateUserAsync(existingUser);
            
            if (!identityResult.Succeeded)
            {
                await _userRepository.RollbackTransactionAsync();
                return Result.Error(string.Join(", ", identityResult.Errors.Select(e => e.Description)));
            }
            await _userRepository.SaveAsync();
            await _userRepository.CommitTransactionAsync();

            return Result<ApplicationUser>.Ok(existingUser);
        }
        catch
        {
            await _userRepository.RollbackTransactionAsync();
            return Result.Error("Unable to create user.");
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
