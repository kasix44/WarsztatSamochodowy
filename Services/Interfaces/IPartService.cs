using System.Collections.Generic;
using System.Threading.Tasks;
using WorkshopManager.Models;

namespace WorkshopManager.Services.Interfaces
{
    public interface IPartService
    {
        Task<List<Part>> GetAllAsync();
        Task<Part?> GetByIdAsync(int id);
        Task CreateAsync(Part part);
        Task UpdateAsync(Part part);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}