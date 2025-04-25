using WorkshopManager.Models;

namespace WorkshopManager.Services.Interfaces
{
    public interface IJobActivityService
    {
        Task<List<JobActivity>> GetAllAsync();
        Task<JobActivity?> GetByIdAsync(int id);
        Task CreateAsync(JobActivity jobActivity);
        Task UpdateAsync(JobActivity jobActivity);
        Task DeleteAsync(int id);
    }
}