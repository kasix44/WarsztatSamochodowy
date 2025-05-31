using Riok.Mapperly.Abstractions;
using WorkshopManager.DTOs;
using WorkshopManager.Models;

namespace WorkshopManager.Mappers;

[Mapper]
public partial class UsedPartMapper
{
    private readonly PartMapper _partMapper;

    [MapperConstructor]
    public UsedPartMapper(PartMapper partMapper)
    {
        _partMapper = partMapper;
    }

    [MapProperty(nameof(UsedPart.Part), nameof(UsedPartDto.Part))]
    [MapProperty(nameof(UsedPart.ServiceOrder), nameof(UsedPartDto.ServiceOrder), Use = nameof(MapServiceOrderToDto))]
    public partial UsedPartDto? ToDto(UsedPart? part);

    [MapProperty(nameof(UsedPartDto.Part), nameof(UsedPart.Part))]
    [MapProperty(nameof(UsedPartDto.ServiceOrderId), nameof(UsedPart.ServiceOrderId))]
    public partial UsedPart? ToEntity(UsedPartDto? dto);

    private PartDto? MapPartToDto(Part? part) => _partMapper.ToDto(part);
    private Part? MapPartFromDto(PartDto? dto) => _partMapper.ToEntity(dto);

    private ServiceOrderDto? MapServiceOrderToDto(ServiceOrder? order)
    {
        if (order == null) return null;
        return new ServiceOrderDto
        {
            Id = order.Id,
            StartDate = order.StartDate,
            EndDate = order.EndDate,
            Status = order.Status,
            VehicleId = order.VehicleId,
            AssignedMechanicId = order.AssignedMechanicId,
            AssignedMechanicUserName = order.AssignedMechanic?.UserName,
            Vehicle = null,
            UsedParts = null,
            JobActivities = null,
            Comments = null
        };
    }
}