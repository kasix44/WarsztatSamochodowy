using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.DTOs;
using WorkshopManager.Mappers;
using WorkshopManager.Models;
using WorkshopManager.Services.Interfaces;

namespace WorkshopManager.Services
{
    public class PartService : IPartService
    {
        private readonly ApplicationDbContext _context;
        private readonly PartMapper _mapper;

        public PartService(ApplicationDbContext context, PartMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PartDto>> GetAllAsync()
        {
            var parts = await _context.Parts.ToListAsync();
            return parts.Select(p => _mapper.ToDto(p)).ToList();
        }

        public async Task<PartDto?> GetByIdAsync(int id)
        {
            var part = await _context.Parts.FindAsync(id);
            return part != null ? _mapper.ToDto(part) : null;
        }

        public async Task<PartDto> AddAsync(PartDto partDto)
        {
            var part = _mapper.ToEntity(partDto);
            _context.Parts.Add(part);
            await _context.SaveChangesAsync();
            return _mapper.ToDto(part);
        }

        public async Task UpdateAsync(PartDto partDto)
        {
            var part = await _context.Parts.FindAsync(partDto.Id);
            if (part == null)
                throw new KeyNotFoundException($"Part with ID {partDto.Id} not found.");

            var updatedPart = _mapper.ToEntity(partDto);
            _context.Entry(part).CurrentValues.SetValues(updatedPart);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part != null)
            {
                _context.Parts.Remove(part);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Parts.AnyAsync(p => p.Id == id);
        }
    }
}