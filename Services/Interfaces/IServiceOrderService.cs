using WorkshopManager.DTOs;

namespace WorkshopManager.Services.Interfaces
{
    public interface IServiceOrderService
    {
        Task<List<ServiceOrderDto>> GetAllAsync();
        Task<ServiceOrderDto?> GetByIdAsync(int id);
        Task<ServiceOrderDto> AddAsync(ServiceOrderDto orderDto);
        Task UpdateAsync(ServiceOrderDto orderDto);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}