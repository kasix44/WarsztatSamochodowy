using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.DTOs;
using WorkshopManager.Mappers;
using WorkshopManager.Models;
using WorkshopManager.Services.Interfaces;

namespace WorkshopManager.Services
{
    public class JobActivityService : IJobActivityService
    {
        private readonly ApplicationDbContext _context;
        private readonly JobActivityMapper _mapper;

        public JobActivityService(ApplicationDbContext context)
        {
            _context = context;
            _mapper = new JobActivityMapper();
        }

        public async Task<List<JobActivityDto>> GetAllAsync()
        {
            var activities = await _context.JobActivities
                .Include(a => a.ServiceOrder)
                .ToListAsync();
            
            return activities.Select(a => _mapper.ToDto(a)).ToList();
        }

        public async Task<JobActivityDto?> GetByIdAsync(int id)
        {
            var activity = await _context.JobActivities
                .Include(a => a.ServiceOrder)
                .FirstOrDefaultAsync(a => a.Id == id);
            
            return activity != null ? _mapper.ToDto(activity) : null;
        }

        public async Task<JobActivityDto> AddAsync(JobActivityDto activityDto)
        {
            var activity = _mapper.ToEntity(activityDto);
            _context.JobActivities.Add(activity);
            await _context.SaveChangesAsync();
            
            var savedActivity = await GetByIdAsync(activity.Id);
            return savedActivity!;
        }

        public async Task UpdateAsync(JobActivityDto activityDto)
        {
            var activity = _mapper.ToEntity(activityDto);
            _context.JobActivities.Update(activity);
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

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.JobActivities.AnyAsync(a => a.Id == id);
        }
    }
}