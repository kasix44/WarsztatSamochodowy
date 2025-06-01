using Microsoft.AspNetCore.Identity;
using Riok.Mapperly.Abstractions;
using WorkshopManager.DTOs;
using WorkshopManager.Models;

namespace WorkshopManager.Mappers;

[Mapper]
public partial class ServiceOrderMapper
{
    private readonly VehicleMapper _vehicleMapper;
    private readonly JobActivityMapper _jobActivityMapper;
    private readonly UsedPartMapper _usedPartMapper;
    private readonly ServiceOrderCommentMapper _commentMapper;

    [MapperConstructor]
    public ServiceOrderMapper(
        VehicleMapper vehicleMapper, 
        JobActivityMapper jobActivityMapper, 
        UsedPartMapper usedPartMapper, 
        ServiceOrderCommentMapper commentMapper
        )
    {
        _vehicleMapper = vehicleMapper;
        _jobActivityMapper = jobActivityMapper;
        _usedPartMapper = usedPartMapper;
        _commentMapper = commentMapper;
    }

    [MapProperty(nameof(ServiceOrder.Vehicle), nameof(ServiceOrderDto.Vehicle))]
    [MapProperty(nameof(ServiceOrder.JobActivities), nameof(ServiceOrderDto.JobActivities))]
    [MapProperty(nameof(ServiceOrder.UsedParts), nameof(ServiceOrderDto.UsedParts))]
    [MapProperty(nameof(ServiceOrder.Comments), nameof(ServiceOrderDto.Comments))]
    [MapProperty(nameof(ServiceOrder.AssignedMechanic), nameof(ServiceOrderDto.AssignedMechanicUserName), Use = nameof(MapAssignedMechanicUserNameFromUser))]
    public partial ServiceOrderDto ToDto(ServiceOrder order);

    [MapProperty(nameof(ServiceOrderDto.Vehicle), nameof(ServiceOrder.Vehicle))]
    [MapProperty(nameof(ServiceOrderDto.JobActivities), nameof(ServiceOrder.JobActivities))]
    [MapProperty(nameof(ServiceOrderDto.UsedParts), nameof(ServiceOrder.UsedParts), Use = nameof(MapUsedPartsCollection))]
    [MapProperty(nameof(ServiceOrderDto.Comments), nameof(ServiceOrder.Comments))]
    
    [MapperIgnoreTarget(nameof(ServiceOrder.AssignedMechanic))]
    [MapperIgnoreSource(nameof(ServiceOrderDto.AssignedMechanicUserName))] 
    public partial ServiceOrder ToEntity(ServiceOrderDto dto);

    private VehicleDto MapVehicleToDto(Vehicle vehicle) => _vehicleMapper.ToDto(vehicle);
    private Vehicle MapVehicleFromDto(VehicleDto dto) => _vehicleMapper.ToEntity(dto);

    private JobActivityDto MapJobActivityToDto(JobActivity activity)
    {
        var dto = _jobActivityMapper.ToDto(activity);
        return dto;
    }

    private JobActivity MapJobActivityFromDto(JobActivityDto dto)
    {
        var activity = _jobActivityMapper.ToEntity(dto);
        return activity;
    }

    private UsedPartDto MapUsedPartToDto(UsedPart part)
    {
        var dto = _usedPartMapper.ToDto(part);
        return dto;
    }

    private UsedPart? MapUsedPartFromDto(UsedPartDto dto)
    {
        var part = _usedPartMapper.ToEntity(dto);
        return part;
    }

    private List<UsedPart>? MapUsedPartsCollection(List<UsedPartDto>? dtos)
    {
        if (dtos == null) return null;
        return dtos.Select(dto => MapUsedPartFromDto(dto)).Where(entity => entity != null).ToList()!;
    }

    private ServiceOrderCommentDto MapServiceOrderCommentToDto(ServiceOrderComment comment)
    {
        var dto = _commentMapper.ToDto(comment);
        return dto;
    }

    private ServiceOrderComment MapServiceOrderCommentFromDto(ServiceOrderCommentDto dto)
    {
        var comment = _commentMapper.ToEntity(dto);
        return comment;
    }

    private string? MapAssignedMechanicUserNameFromUser(IdentityUser? user) => user?.UserName;
} 