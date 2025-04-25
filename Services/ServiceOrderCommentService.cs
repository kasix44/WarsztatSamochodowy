using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.Models;
using WorkshopManager.Services.Interfaces;

namespace WorkshopManager.Services
{
    public class ServiceOrderCommentService : IServiceOrderCommentService
    {
        private readonly ApplicationDbContext _context;

        public ServiceOrderCommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceOrderComment>> GetByOrderIdAsync(int serviceOrderId)
        {
            return await _context.ServiceOrderComments
                .Where(c => c.ServiceOrderId == serviceOrderId)
                .ToListAsync();
        }

        public async Task<ServiceOrderComment?> GetByIdAsync(int id)
        {
            return await _context.ServiceOrderComments.FindAsync(id);
        }

        public async Task AddAsync(ServiceOrderComment comment)
        {
            _context.ServiceOrderComments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ServiceOrderComment comment)
        {
            _context.ServiceOrderComments.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var comment = await _context.ServiceOrderComments.FindAsync(id);
            if (comment != null)
            {
                _context.ServiceOrderComments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public bool Exists(int id)
        {
            return _context.ServiceOrderComments.Any(c => c.Id == id);
        }
    }
}