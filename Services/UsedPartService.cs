using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.DTOs;
using WorkshopManager.Mappers;
using WorkshopManager.Models;
using WorkshopManager.Services.Interfaces;

namespace WorkshopManager.Services
{
    public class UsedPartService : IUsedPartService
    {
        private readonly ApplicationDbContext _context;
        private readonly UsedPartMapper _mapper;

        public UsedPartService(ApplicationDbContext context, UsedPartMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UsedPartDto> AddAsync(UsedPartDto usedPartDto)
        {
            // Ensure we're not setting an ID for new entities
            usedPartDto.Id = 0;

            var usedPart = _mapper.ToEntity(usedPartDto);
            _context.UsedParts.Add(usedPart);
            await _context.SaveChangesAsync();

            // Reload the used part with its relationships
            var savedUsedPart = await _context.UsedParts
                .Include(up => up.Part)
                .FirstOrDefaultAsync(up => up.Id == usedPart.Id);

            return _mapper.ToDto(savedUsedPart);
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