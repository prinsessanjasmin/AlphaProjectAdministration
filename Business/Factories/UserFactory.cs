using Business.Models;
using Data.Entities;

namespace Business.Factories;

public static class UserFactory
{
    public static ApplicationUser Create (AppUserDto model)
    {
        var appUser = new ApplicationUser
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            UserName = model.Email,
        };

        return appUser;
    }

    public static AppUserDto Create(ApplicationUser appUser)
    {
        return new AppUserDto
        {
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            Email = appUser.Email ?? "Not provided"
        };
    }

    public static ApplicationUser Create(UpdateUserDto model, int id)
    {
        var appUser = new ApplicationUser
        {
            PhoneNumber = model.PhoneNumber ?? "",
            JobTitle = model.JobTitle,
            ProfileImagePath = model.ProfileImagePath,
            DateOfBirth = model.DateOfBirth,
            AddressId = id
        };

        return appUser;
    }

    public static ApplicationUser Create(EmployeeDto model, int id)
    {
        var appUser = new ApplicationUser
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            UserName = model.Email,
            PhoneNumber = model.PhoneNumber ?? "",
            JobTitle = model.JobTitle,
            ProfileImagePath = model.ProfileImagePath,
            DateOfBirth = model.DateOfBirth,
            AddressId = id
        };

        return appUser;
    }



    public static ApplicationUser Create(EmployeeDto model)
    {
        var appUser = new ApplicationUser
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            UserName = model.Email,
            PhoneNumber = model.PhoneNumber ?? "",
            JobTitle = model.JobTitle,
            ProfileImagePath = model.ProfileImagePath,
            DateOfBirth = model.DateOfBirth,
        };

        return appUser;
    }


    public static ApplicationUser Create(EditEmployeeDto model)
    {
        var appUser = new ApplicationUser
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            UserName = model.Email,
            PhoneNumber = model.PhoneNumber ?? "",
            JobTitle = model.JobTitle,
            ProfileImagePath = model.ProfileImagePath,
            DateOfBirth = model.DateOfBirth,
        };

        return appUser;
    }

}
