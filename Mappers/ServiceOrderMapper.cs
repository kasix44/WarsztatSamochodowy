using Microsoft.AspNetCore.Identity;
using Riok.Mapperly.Abstractions;
using WorkshopManager.DTOs;
using WorkshopManager.Models;

namespace WorkshopManager.Mappers;

[Mapper]
public partial class ServiceOrderMapper
{
    private readonly VehicleMapper _vehicleMapper = new();
    private readonly JobActivityMapper _jobActivityMapper = new();
    private readonly UsedPartMapper _usedPartMapper = new();
    private readonly ServiceOrderCommentMapper _commentMapper = new();

    [MapperConstructor]
    public ServiceOrderMapper()
    {
    }

    [MapProperty(nameof(ServiceOrder.Vehicle), nameof(ServiceOrderDto.Vehicle))]
    [MapProperty(nameof(ServiceOrder.JobActivities), nameof(ServiceOrderDto.JobActivities))]
    [MapProperty(nameof(ServiceOrder.UsedParts), nameof(ServiceOrderDto.UsedParts))]
    [MapProperty(nameof(ServiceOrder.Comments), nameof(ServiceOrderDto.Comments))]
    [MapProperty(nameof(ServiceOrder.AssignedMechanic), nameof(ServiceOrderDto.AssignedMechanicUserName), Use = nameof(MapAssignedMechanicUserName))]
    public partial ServiceOrderDto ToDto(ServiceOrder order);

    [MapProperty(nameof(ServiceOrderDto.Vehicle), nameof(ServiceOrder.Vehicle))]
    [MapProperty(nameof(ServiceOrderDto.JobActivities), nameof(ServiceOrder.JobActivities))]
    [MapProperty(nameof(ServiceOrderDto.UsedParts), nameof(ServiceOrder.UsedParts))]
    [MapProperty(nameof(ServiceOrderDto.Comments), nameof(ServiceOrder.Comments))]
    public partial ServiceOrder ToEntity(ServiceOrderDto dto);

    private VehicleDto MapVehicle(Vehicle vehicle) => _vehicleMapper.ToDto(vehicle);
    private Vehicle MapVehicle(VehicleDto dto) => _vehicleMapper.ToEntity(dto);

    private JobActivityDto MapJobActivity(JobActivity activity)
    {
        var dto = _jobActivityMapper.ToDto(activity);
        dto.ServiceOrder = null; // Prevent circular reference
        return dto;
    }

    private JobActivity MapJobActivity(JobActivityDto dto)
    {
        var activity = _jobActivityMapper.ToEntity(dto);
        activity.ServiceOrder = null; // Prevent circular reference
        return activity;
    }

    private UsedPartDto MapUsedPart(UsedPart part)
    {
        var dto = _usedPartMapper.ToDto(part);
        dto.ServiceOrder = null; // Prevent circular reference
        return dto;
    }

    private UsedPart MapUsedPart(UsedPartDto dto)
    {
        var part = _usedPartMapper.ToEntity(dto);
        part.ServiceOrder = null; // Prevent circular reference
        return part;
    }

    private ServiceOrderCommentDto MapServiceOrderComment(ServiceOrderComment comment)
    {
        var dto = _commentMapper.ToDto(comment);
        dto.ServiceOrder = null; // Prevent circular reference
        return dto;
    }

    private ServiceOrderComment MapServiceOrderComment(ServiceOrderCommentDto dto)
    {
        var comment = _commentMapper.ToEntity(dto);
        comment.ServiceOrder = null; // Prevent circular reference
        return comment;
    }

    private string? MapAssignedMechanicUserName(IdentityUser? user) => user?.UserName;
} 