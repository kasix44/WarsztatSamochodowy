using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.Models;
using WorkshopManager.Services.Interfaces;

namespace WorkshopManager.Services
{
    public class ServiceOrderService : IServiceOrderService
    {
        private readonly ApplicationDbContext _context;

        public ServiceOrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceOrder>> GetAllAsync()
        {
            return await _context.ServiceOrders
                .Include(o => o.Vehicle)
                .Include(o => o.AssignedMechanic)
                .ToListAsync();
        }

        public async Task<ServiceOrder?> GetByIdAsync(int id)
        {
            return await _context.ServiceOrders
                .Include(o => o.Vehicle)
                .Include(o => o.AssignedMechanic)
                .Include(o => o.UsedParts).ThenInclude(p => p.Part)
                .Include(o => o.JobActivities)
                .Include(o => o.Comments)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task AddAsync(ServiceOrder order)
        {
            _context.ServiceOrders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ServiceOrder order)
        {
            _context.ServiceOrders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _context.ServiceOrders.FindAsync(id);
            if (order != null)
            {
                _context.ServiceOrders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        public bool Exists(int id)
        {
            return _context.ServiceOrders.Any(e => e.Id == id);
        }
    }
}