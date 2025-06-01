using Riok.Mapperly.Abstractions;
using WorkshopManager.DTOs;
using WorkshopManager.Models;
using System; // Required for Lazy<T>

namespace WorkshopManager.Mappers;

[Mapper]
public partial class VehicleMapper
{
    private readonly Lazy<CustomerMapper> _customerMapperLazy;

    [MapperConstructor]
    public VehicleMapper(Lazy<CustomerMapper> customerMapperLazy)
    {
        _customerMapperLazy = customerMapperLazy;
    }

    [MapProperty(nameof(Vehicle.Customer), nameof(VehicleDto.Customer), Use = nameof(MapToShallowCustomerDto))]
    public partial VehicleDto? ToDto(Vehicle? vehicle);
    
    [MapProperty(nameof(VehicleDto.Customer), nameof(Vehicle.Customer), Use = nameof(MapToShallowCustomerEntity))]
    public partial Vehicle? ToEntity(VehicleDto? dto);
    
    public CustomerDto? MapToShallowCustomerDto(Customer? customer)
    {
        if (customer == null) return null;
        return _customerMapperLazy.Value.ToShallowDto(customer);
    }
    
    public Customer? MapToShallowCustomerEntity(CustomerDto? customerDto)
    {
        if (customerDto == null) return null;
        return _customerMapperLazy.Value.ToShallowEntity(customerDto);
    }

} 