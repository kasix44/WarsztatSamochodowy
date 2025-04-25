using Riok.Mapperly.Abstractions;
using WorkshopManager.DTOs;
using WorkshopManager.Models;

namespace WorkshopManager.Mappers;

[Mapper]
public partial class UsedPartMapper
{
    private readonly PartMapper _partMapper = new();

    [MapperConstructor]
    public UsedPartMapper()
    {
    }

    [MapProperty(nameof(UsedPart.Part), nameof(UsedPartDto.Part))]
    [MapProperty(nameof(UsedPart.ServiceOrder), nameof(UsedPartDto.ServiceOrder))]
    public partial UsedPartDto ToDto(UsedPart part);

    [MapProperty(nameof(UsedPartDto.Part), nameof(UsedPart.Part))]
    [MapProperty(nameof(UsedPartDto.ServiceOrder), nameof(UsedPart.ServiceOrder))]
    public partial UsedPart ToEntity(UsedPartDto dto);

    private PartDto MapPart(Part part) => _partMapper.ToDto(part);
    private Part MapPart(PartDto dto) => _partMapper.ToEntity(dto);

    private ServiceOrderDto MapServiceOrder(ServiceOrder order)
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
            // Don't map collections to prevent circular reference
            Vehicle = null,
            UsedParts = null,
            JobActivities = null,
            Comments = null
        };
    }

    private ServiceOrder MapServiceOrder(ServiceOrderDto dto)
    {
        if (dto == null) return null;
        return new ServiceOrder
        {
            Id = dto.Id,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = dto.Status,
            VehicleId = dto.VehicleId,
            AssignedMechanicId = dto.AssignedMechanicId,
            // Don't map collections to prevent circular reference
            Vehicle = null,
            UsedParts = null,
            JobActivities = null,
            Comments = null
        };
    }

    private UsedPart MapUsedPart(UsedPartDto dto)
    {
        if (dto == null) return null;
        return new UsedPart
        {
            // Don't set Id for new entities
            Id = dto.Id == 0 ? 0 : dto.Id,
            PartId = dto.PartId,
            Quantity = dto.Quantity,
            ServiceOrderId = dto.ServiceOrderId
        };
    }
} 