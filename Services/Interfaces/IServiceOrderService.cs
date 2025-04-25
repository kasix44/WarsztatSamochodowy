using WorkshopManager.Models;

namespace WorkshopManager.Services.Interfaces
{
    public interface IServiceOrderService
    {
        Task<List<ServiceOrder>> GetAllAsync();
        Task<ServiceOrder?> GetByIdAsync(int id);
        Task AddAsync(ServiceOrder order);
        Task UpdateAsync(ServiceOrder order);
        Task DeleteAsync(int id);
        bool Exists(int id);
    }
}