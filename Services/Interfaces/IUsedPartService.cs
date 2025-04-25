using System.Threading.Tasks;
using WorkshopManager.Models;

namespace WorkshopManager.Services.Interfaces
{
    public interface IUsedPartService
    {
        Task AddAsync(UsedPart usedPart);
        Task DeleteAsync(int id);
    }
}