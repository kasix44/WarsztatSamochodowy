using Riok.Mapperly.Abstractions;
using WorkshopManager.DTOs;
using WorkshopManager.Models;

namespace WorkshopManager.Mappers;

[Mapper]
public partial class VehicleMapper
{
    [MapProperty(nameof(Vehicle.Customer), nameof(VehicleDto.Customer))]
    public partial VehicleDto ToDto(Vehicle vehicle);
    
    [MapProperty(nameof(VehicleDto.Customer), nameof(Vehicle.Customer))]
    public partial Vehicle ToEntity(VehicleDto dto);
    
    private CustomerDto MapCustomer(Customer customer)
    {
        if (customer == null) return null;
        return new CustomerDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber,
            Email = customer.Email,
            Address = customer.Address,
            // Don't map Vehicles here to prevent circular reference
            Vehicles = null
        };
    }
    
    private Customer MapCustomer(CustomerDto dto)
    {
        if (dto == null) return null;
        return new Customer
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            Address = dto.Address,
            // Don't map Vehicles here to prevent circular reference
            Vehicles = null
        };
    }
} 