using Business.Models;
using Data.Entities;

namespace Business.Factories;

public static class EmployeeFactory
{
    public static EmployeeEntity Create (MemberDto form, int id)
    {
        var employeeEntity = new EmployeeEntity
        {
            FirstName = form.FirstName,
            LastName = form.LastName,
            Email = form.Email,
            PhoneNumber = form.PhoneNumber ?? "",
            JobTitle = form.JobTitle,
            ProfileImagePath = form.ProfileImagePath,
            DateOfBirth = form.DateOfBirth,
            AddressId = id
        };



            return employeeEntity;
    }

    public static MemberDto Create(EmployeeEntity employee, AddressEntity address)
    {
        return new MemberDto
        {
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            PhoneNumber = employee.PhoneNumber,
            JobTitle = employee.JobTitle,
            ProfileImagePath = employee.ProfileImagePath,
            DateOfBirth = employee.DateOfBirth,
            StreetAddress = address.StreetAddress,
            PostCode = address.PostCode,
            City = address.City,
        };
    }
}
