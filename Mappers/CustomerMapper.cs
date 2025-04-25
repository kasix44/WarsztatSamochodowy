using Riok.Mapperly.Abstractions;
using WorkshopManager.DTOs;
using WorkshopManager.Models;

namespace WorkshopManager.Mappers;

[Mapper]
public partial class CustomerMapper
{
    private readonly VehicleMapper _vehicleMapper = new();

    [MapperConstructor]
    public CustomerMapper()
    {
    }

    [MapProperty(nameof(Customer.Vehicles), nameof(CustomerDto.Vehicles))]
    public partial CustomerDto ToDto(Customer customer);

    [MapProperty(nameof(CustomerDto.Vehicles), nameof(Customer.Vehicles))]
    public partial Customer ToEntity(CustomerDto dto);

    private VehicleDto MapVehicle(Vehicle vehicle)
    {
        if (vehicle == null) return null;
        var dto = _vehicleMapper.ToDto(vehicle);
        // Don't map Customer here to prevent circular reference
        dto.Customer = null;
        return dto;
    }

    private Vehicle MapVehicle(VehicleDto vehicleDto)
    {
        if (vehicleDto == null) return null;
        var entity = _vehicleMapper.ToEntity(vehicleDto);
        // Don't map Customer here to prevent circular reference
        entity.Customer = null;
        return entity;
    }

    // You can also add custom mapping methods if needed:
    // [MapProperty(nameof(CustomerDto.FirstName), nameof(Customer.FirstName))]
    // public partial Customer ToEntityWithCustomMapping(CustomerDto dto);
} 