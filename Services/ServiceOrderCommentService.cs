using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.DTOs;
using WorkshopManager.Mappers;
using WorkshopManager.Models;
using WorkshopManager.Services.Interfaces;

namespace WorkshopManager.Services
{
    public class ServiceOrderCommentService : IServiceOrderCommentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ServiceOrderCommentMapper _mapper;

        public ServiceOrderCommentService(ApplicationDbContext context, ServiceOrderCommentMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ServiceOrderCommentDto>> GetByOrderIdAsync(int serviceOrderId)
        {
            var comments = await _context.ServiceOrderComments
                .Where(c => c.ServiceOrderId == serviceOrderId)
                .ToListAsync();
            
            return comments.Select(c => _mapper.ToDto(c)).ToList();
        }

        public async Task<ServiceOrderCommentDto?> GetByIdAsync(int id)
        {
            var comment = await _context.ServiceOrderComments.FindAsync(id);
            return comment != null ? _mapper.ToDto(comment) : null;
        }

        public async Task<ServiceOrderCommentDto> AddAsync(ServiceOrderCommentDto commentDto)
        {
            var comment = _mapper.ToEntity(commentDto);
            _context.ServiceOrderComments.Add(comment);
            await _context.SaveChangesAsync();
            return _mapper.ToDto(comment);
        }

        public async Task UpdateAsync(ServiceOrderCommentDto commentDto)
        {
            var comment = _mapper.ToEntity(commentDto);
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

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ServiceOrderComments.AnyAsync(c => c.Id == id);
        }
    }
}