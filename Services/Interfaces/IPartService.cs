using System.Collections.Generic;
using System.Threading.Tasks;
using WorkshopManager.DTOs;

namespace WorkshopManager.Services.Interfaces
{
    public interface IPartService
    {
        Task<List<PartDto>> GetAllAsync();
        Task<PartDto?> GetByIdAsync(int id);
        Task<PartDto> AddAsync(PartDto partDto);
        Task UpdateAsync(PartDto partDto);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}