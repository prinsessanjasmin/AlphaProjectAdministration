using Business.Models;
using Data.Entities;

namespace Business.Factories;

public static class EmployeeFactory
{
    public static EmployeeEntity Create (MemberFormModel form, int id)
    {
        var employeeEntity = new EmployeeEntity
        {
            FirstName = form.FirstName,
            LastName = form.LastName,
            Email = form.Email,
            PhoneNumber = form.PhoneNumber,
            JobTitle = form.JobTitle,
            ProfileImage = form.ProfileImage,
            DateOfBirth = form.DateOfBirth,
            AddressId = id
        };



            return employeeEntity;
    }

    public static MemberFormModel Create(EmployeeEntity employee, AddressEntity address)
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
            StreetAddress = address.StreetAddress,
            PostCode = address.PostCode,
            City = address.City,
        };
    }
}
