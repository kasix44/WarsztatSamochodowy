using System.Threading.Tasks;
using WorkshopManager.DTOs;

namespace WorkshopManager.Services.Interfaces
{
    public interface IUsedPartService
    {
        Task<UsedPartDto> AddAsync(UsedPartDto usedPartDto);
        Task DeleteAsync(int id);
    }
}