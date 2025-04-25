using WorkshopManager.DTOs;
using WorkshopManager.Models;

namespace WorkshopManager.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<List<CustomerDto>> GetAllAsync(string? search = null);
        Task<CustomerDto?> GetByIdAsync(int id);
        Task AddAsync(CustomerDto customerDto);
        Task UpdateAsync(CustomerDto customerDto);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}