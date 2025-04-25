using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.Models;
using WorkshopManager.Services.Interfaces;

namespace WorkshopManager.Services
{
    public class JobActivityService : IJobActivityService
    {
        private readonly ApplicationDbContext _context;

        public JobActivityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<JobActivity>> GetAllAsync()
        {
            return await _context.JobActivities.ToListAsync();
        }

        public async Task<JobActivity?> GetByIdAsync(int id)
        {
            return await _context.JobActivities.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task CreateAsync(JobActivity jobActivity)
        {
            _context.JobActivities.Add(jobActivity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(JobActivity jobActivity)
        {
            _context.JobActivities.Update(jobActivity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var activity = await _context.JobActivities.FindAsync(id);
            if (activity != null)
            {
                _context.JobActivities.Remove(activity);
                await _context.SaveChangesAsync();
            }
        }
    }
}