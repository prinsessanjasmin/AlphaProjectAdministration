using Business.Models;
using Data.Entities;

namespace Business.Factories;

public static class EmployeeFactory
{
    public static EmployeeEntity Create (MemberFormModel form)
    {
        return new EmployeeEntity
        {
            FirstName = form.FirstName,
            LastName = form.LastName,
            Email = form.Email,
            PhoneNumber = form.PhoneNumber,
            JobTitle = form.JobTitle,
            ProfileImage = form.ProfileImage,
            DateOfBirth = form.DateOfBirth,
            StreetAddress = form.StreetAddress,
            PostCode = form.PostCode,
            City = form.City,
        };
    }

    public static MemberFormModel Create(EmployeeEntity employee)
    {
        return new MemberFormModel
        {
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            PhoneNumber = employee.PhoneNumber,
            JobTitle = employee.JobTitle,
            ProfileImage = employee.ProfileImage,
            DateOfBirth = employee.DateOfBirth,
            StreetAddress = employee.StreetAddress,
            PostCode = employee.PostCode,
            City = employee.City,
        };
    }
}
