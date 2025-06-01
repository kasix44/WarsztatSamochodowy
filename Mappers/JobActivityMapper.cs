using Riok.Mapperly.Abstractions;
using WorkshopManager.DTOs;
using WorkshopManager.Models;

namespace WorkshopManager.Mappers;

[Mapper]
public partial class JobActivityMapper
{
    public partial JobActivityDto? ToDto(JobActivity? activity);

    public partial JobActivity? ToEntity(JobActivityDto? dto);
} 