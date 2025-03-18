using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;

namespace Business.Services;

public class AddressService(IAddressRepository addressRepository) : IAddressService
{
    private readonly IAddressRepository _addressRepository = addressRepository;

    public async Task<IResult<AddressEntity>> CreateAddress(MemberFormModel form)
    {
        await _addressRepository.BeginTransactionAsync();
        try
        {
            if (form.StreetAddress == null || form.PostCode == null || form.City == null)
            {
                 return Result<AddressEntity>.Error("You need to fill out all address fields.");
            }

            string normailizedStreetAddress = form.StreetAddress.Trim().ToLower();
            string normalizedPostCode = form.PostCode.Trim();
            string normalizedCity = form.City.Trim().ToLower();
            
            var existingAddress = await _addressRepository.GetAsync(a =>
                a.StreetAddress == normailizedStreetAddress
                && a.PostCode == normalizedPostCode
                && a.City == normalizedCity);

            if (existingAddress != null) 
            {
                return Result<AddressEntity>.Ok(existingAddress); 
            }
            else
            {
                AddressEntity addressEntity = AddressFactory.Create(normailizedStreetAddress, normalizedPostCode, normalizedCity);
                await _addressRepository.CreateAsync(addressEntity);
                await _addressRepository.SaveAsync();
                await _addressRepository.CommitTransactionAsync();
                return Result<AddressEntity>.Created(addressEntity); 
            }
        }
        catch
        {
            await _addressRepository.RollbackTransactionAsync();
            return Result<AddressEntity>.Error("Something went wrong.");
        }
    }



    public async Task<IResult> GetAllAddresses()
    {
        IEnumerable<AddressEntity> addresses = new List<AddressEntity>();

        try
        {
            addresses = await _addressRepository.GetAsync();
            if (addresses == null)
            {
                return Result.NotFound("There are no addresses registered.");
            }

            return Result<IEnumerable<AddressEntity>>.Ok(addresses);
        }
        catch
        {
            return Result.Error("Something went wrong");
        }
    }

    public async Task<IResult> GetAddressByAddress(string streetAddress, string postCode, string city)
    {
        try
        {
            AddressEntity address = await _addressRepository.GetAsync(a => a.StreetAddress == streetAddress && a.PostCode == postCode && a.City == city );
            if (address == null)
            {
                return Result.NotFound("This address does not exist in the database yet.");
            }

            return Result<AddressEntity>.Ok(address);
        }
        catch
        {
            return Result.Error("Something went wrong");
        }
    }


    public async Task<IResult> UpdateAddress(int id, AddressEntity updatedAddress)
    {
        await _addressRepository.BeginTransactionAsync();
        try
        {
            AddressEntity address = await _addressRepository.UpdateAsync(a => a.Id == id, updatedAddress);
            if (address == null)
                return Result.NotFound("Could not find the address in the database.");

            await _addressRepository.SaveAsync();
            await _addressRepository.CommitTransactionAsync();
            return Result<AddressEntity>.Ok(address);
        }
        catch
        {
            return Result.Error("Something went wrong");
        }
    }

    public async Task<IResult> DeleteAddress(int id)
    {
        await _addressRepository.BeginTransactionAsync();
        try
        {
            bool deleted = await _addressRepository.DeleteAsync(a => a.Id == id);
            if (deleted)
            {
                await _addressRepository.SaveAsync();
                await _addressRepository.CommitTransactionAsync();
                return Result.Ok();
            }

            return Result.Error("Something went wrong.");

        }
        catch
        {
            await _addressRepository.RollbackTransactionAsync();
            return Result.Error("Something went wrong.");
        }
    }
}

