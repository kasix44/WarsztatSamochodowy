using Riok.Mapperly.Abstractions;
using WorkshopManager.DTOs;
using WorkshopManager.Models;
using System; // Required for Lazy<T>
using System.Collections.Generic;
using System.Linq;

namespace WorkshopManager.Mappers;

[Mapper]
public partial class CustomerMapper
{
    private readonly Lazy<VehicleMapper> _vehicleMapperLazy;

    [MapperConstructor]
    public CustomerMapper(Lazy<VehicleMapper> vehicleMapperLazy)
    {
        _vehicleMapperLazy = vehicleMapperLazy;
    }

    [UserMapping(Default = true)]
    [MapProperty(nameof(Customer.Vehicles), nameof(CustomerDto.Vehicles), Use = nameof(MapVehicleCollectionToDto))]
    public partial CustomerDto? ToDto(Customer? customer);

    [UserMapping(Default = true)]
    // Assuming similar logic might be needed for ToEntity if VehicleDto also has a Customer property
    // that could cause a loop. For now, focusing on ToDto as per stack trace.
    [MapProperty(nameof(CustomerDto.Vehicles), nameof(Customer.Vehicles), Use = nameof(MapVehicleDtoCollectionToEntity))]
    public partial Customer? ToEntity(CustomerDto? dto);

    // Custom method to map a collection of Vehicle entities to VehicleDto, using VehicleMapper
    private List<VehicleDto>? MapVehicleCollectionToDto(List<Vehicle>? vehicles)
    {
        if (vehicles == null) return null;
        // Ensure _vehicleMapperLazy.Value is accessed here to get the actual VehicleMapper instance.
        return vehicles.Select(v => _vehicleMapperLazy.Value.ToDto(v)).ToList();
    }

    // Custom method to map a collection of VehicleDto to Vehicle entities, using VehicleMapper
    private List<Vehicle>? MapVehicleDtoCollectionToEntity(List<VehicleDto>? vehicleDtos)
    {
        if (vehicleDtos == null) return null;
        return vehicleDtos.Select(dto => _vehicleMapperLazy.Value.ToEntity(dto)).ToList();
    }

    // New shallow DTO mapping method
    public CustomerDto? ToShallowDto(Customer? customer)
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
            Vehicles = null // Explicitly do not map vehicles here
        };
    }

    // New shallow Entity mapping method
    public Customer? ToShallowEntity(CustomerDto? customerDto)
    {
        if (customerDto == null) return null;
        return new Customer
        {
            Id = customerDto.Id,
            FirstName = customerDto.FirstName,
            LastName = customerDto.LastName,
            PhoneNumber = customerDto.PhoneNumber,
            Email = customerDto.Email,
            Address = customerDto.Address,
            Vehicles = null // Explicitly do not map vehicles here
        };
    }

    // You can also add custom mapping methods if needed:
    // [MapProperty(nameof(CustomerDto.FirstName), nameof(Customer.FirstName))]
    // public partial Customer ToEntityWithCustomMapping(CustomerDto dto);
} 