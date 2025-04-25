using Microsoft.AspNetCore.Identity;
using Riok.Mapperly.Abstractions;
using WorkshopManager.DTOs;
using WorkshopManager.Models;

namespace WorkshopManager.Mappers;

[Mapper]
public partial class ServiceOrderCommentMapper
{
    [MapProperty(nameof(ServiceOrderComment.ServiceOrder), nameof(ServiceOrderCommentDto.ServiceOrder))]
    public partial ServiceOrderCommentDto ToDto(ServiceOrderComment comment);

    [MapProperty(nameof(ServiceOrderCommentDto.ServiceOrder), nameof(ServiceOrderComment.ServiceOrder))]
    public partial ServiceOrderComment ToEntity(ServiceOrderCommentDto dto);

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

    private string? MapAuthorUserName(IdentityUser? user) => user?.UserName;
} 