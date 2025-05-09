﻿using Business.Models;
using Data.Entities;

namespace Business.Interfaces; 

public interface IAddressService
{
    Task<IResult<AddressEntity>> CreateAddress(EditEmployeeDto form);
    Task<IResult<AddressEntity>> CreateAddress(UpdateUserDto form);
    Task<IResult<AddressEntity>> CreateAddress(EmployeeDto form);
    Task<IResult> GetAllAddresses();
    Task<IResult> GetAddressByAddress(string streetAddress, string postCode, string city);
    Task<IResult> UpdateAddress(int id, AddressEntity updatedAddress);
    Task<IResult> DeleteAddress(int id); 
}
