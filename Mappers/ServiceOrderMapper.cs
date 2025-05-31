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
    // private readonly UserManager<IdentityUser> _userManager; // If needed for MapAssignedMechanicUserName related logic, though current implementation doesn't use it beyond mapping a direct property.

    [MapperConstructor]
    public ServiceOrderMapper(
        VehicleMapper vehicleMapper, 
        JobActivityMapper jobActivityMapper, 
        UsedPartMapper usedPartMapper, 
        ServiceOrderCommentMapper commentMapper
        // UserManager<IdentityUser> userManager // Uncomment if needed
        )
    {
        _vehicleMapper = vehicleMapper;
        _jobActivityMapper = jobActivityMapper;
        _usedPartMapper = usedPartMapper;
        _commentMapper = commentMapper;
        // _userManager = userManager; // Uncomment if needed
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
    // How to map AssignedMechanicUserName back to AssignedMechanicId or IdentityUser?
    // This requires UserManager or a similar service to fetch user by name.
    // For now, we might need to ignore this field on ToEntity or handle it in the service layer.
    // Let's assume AssignedMechanicId is set directly in the service if UserName is provided.
    [MapperIgnoreTarget(nameof(ServiceOrder.AssignedMechanic))]
    [MapperIgnoreSource(nameof(ServiceOrderDto.AssignedMechanicUserName))] // Or handle if possible
    public partial ServiceOrder ToEntity(ServiceOrderDto dto);

    // Vehicle mapping uses the injected _vehicleMapper
    private VehicleDto MapVehicleToDto(Vehicle vehicle) => _vehicleMapper.ToDto(vehicle);
    private Vehicle MapVehicleFromDto(VehicleDto dto) => _vehicleMapper.ToEntity(dto);

    // JobActivity mapping uses the injected _jobActivityMapper
    private JobActivityDto MapJobActivityToDto(JobActivity activity)
    {
        var dto = _jobActivityMapper.ToDto(activity);
        // dto.ServiceOrder = null; // Already handled by JobActivityMapper's shallow mapping if it exists
        return dto;
    }

    private JobActivity MapJobActivityFromDto(JobActivityDto dto)
    {
        var activity = _jobActivityMapper.ToEntity(dto);
        // activity.ServiceOrder = null; // Already handled by JobActivityMapper's shallow mapping
        return activity;
    }

    // UsedPart mapping uses the injected _usedPartMapper
    private UsedPartDto MapUsedPartToDto(UsedPart part)
    {
        var dto = _usedPartMapper.ToDto(part);
        // dto.ServiceOrder = null; // Should be handled by UsedPartMapper's shallow mapping
        return dto;
    }

    private UsedPart? MapUsedPartFromDto(UsedPartDto dto)
    {
        var part = _usedPartMapper.ToEntity(dto);
        // if (part != null) part.ServiceOrder = null; // Should be handled by UsedPartMapper's shallow mapping
        return part;
    }

    private List<UsedPart>? MapUsedPartsCollection(List<UsedPartDto>? dtos)
    {
        if (dtos == null) return null;
        return dtos.Select(dto => MapUsedPartFromDto(dto)).Where(entity => entity != null).ToList()!;
    }

    // ServiceOrderComment mapping uses the injected _commentMapper
    private ServiceOrderCommentDto MapServiceOrderCommentToDto(ServiceOrderComment comment)
    {
        var dto = _commentMapper.ToDto(comment);
        // dto.ServiceOrder = null; // Should be handled by ServiceOrderCommentMapper's shallow mapping
        return dto;
    }

    private ServiceOrderComment MapServiceOrderCommentFromDto(ServiceOrderCommentDto dto)
    {
        var comment = _commentMapper.ToEntity(dto);
        // comment.ServiceOrder = null; // Should be handled by ServiceOrderCommentMapper's shallow mapping
        return comment;
    }

    // Renamed to avoid conflict and clarify it's for ToDto
    private string? MapAssignedMechanicUserNameFromUser(IdentityUser? user) => user?.UserName;
} 