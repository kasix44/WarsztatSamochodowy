using WorkshopManager.DTOs;

namespace WorkshopManager.Services.Interfaces
{
    public interface IJobActivityService
    {
        Task<List<JobActivityDto>> GetAllAsync();
        Task<JobActivityDto?> GetByIdAsync(int id);
        Task<JobActivityDto> AddAsync(JobActivityDto activityDto);
        Task UpdateAsync(JobActivityDto activityDto);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}