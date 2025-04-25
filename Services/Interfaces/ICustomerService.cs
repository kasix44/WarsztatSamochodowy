using WorkshopManager.Models;

namespace WorkshopManager.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllAsync(string? search = null);
        Task<Customer?> GetByIdAsync(int id);
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}