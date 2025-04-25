using System.Threading.Tasks;
using WorkshopManager.Data;
using WorkshopManager.Models;
using WorkshopManager.Services.Interfaces;

namespace WorkshopManager.Services
{
    public class UsedPartService : IUsedPartService
    {
        private readonly ApplicationDbContext _context;

        public UsedPartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UsedPart usedPart)
        {
            _context.UsedParts.Add(usedPart);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var usedPart = await _context.UsedParts.FindAsync(id);
            if (usedPart != null)
            {
                _context.UsedParts.Remove(usedPart);
                await _context.SaveChangesAsync();
            }
        }
    }
}