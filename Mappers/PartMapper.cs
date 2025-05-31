using Riok.Mapperly.Abstractions;
using WorkshopManager.DTOs;
using WorkshopManager.Models;

namespace WorkshopManager.Mappers;

[Mapper]
public partial class PartMapper
{
    public partial PartDto? ToDto(Part? part);
    public partial Part? ToEntity(PartDto? dto);
} 