using Riok.Mapperly.Abstractions;
using WorkshopManager.DTOs;
using WorkshopManager.Models;

namespace WorkshopManager.Mappers;

[Mapper]
public partial class PartMapper
{
    public partial PartDto ToDto(Part part);
    public partial Part ToEntity(PartDto dto);

    private PartDto MapPart(Part part)
    {
        if (part == null) return null;
        return new PartDto
        {
            Id = part.Id,
            Name = part.Name,
            UnitPrice = part.UnitPrice
        };
    }

    private Part MapPart(PartDto dto)
    {
        if (dto == null) return null;
        return new Part
        {
            Id = dto.Id,
            Name = dto.Name,
            UnitPrice = dto.UnitPrice
        };
    }
} 