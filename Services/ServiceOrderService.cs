using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.DTOs;
using WorkshopManager.Mappers;
using WorkshopManager.Models;
using WorkshopManager.Services.Interfaces;

namespace WorkshopManager.Services
{
    public class ServiceOrderService : IServiceOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ServiceOrderMapper _mapper;

        public ServiceOrderService(ApplicationDbContext context, ServiceOrderMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ServiceOrderDto>> GetAllAsync()
        {
            var orders = await _context.ServiceOrders
                .Include(o => o.Vehicle)
                .Include(o => o.AssignedMechanic)
                .Include(o => o.UsedParts)
                    .ThenInclude(up => up.Part)
                .Include(o => o.JobActivities)
                .Include(o => o.Comments)
                .ToListAsync();

            return orders.Select(o => _mapper.ToDto(o)).ToList();
        }

        public async Task<ServiceOrderDto?> GetByIdAsync(int id)
        {
            var order = await _context.ServiceOrders
                .Include(o => o.Vehicle)
                .Include(o => o.AssignedMechanic)
                .Include(o => o.UsedParts)
                    .ThenInclude(up => up.Part)
                .Include(o => o.JobActivities)
                .Include(o => o.Comments)
                .FirstOrDefaultAsync(o => o.Id == id);

            return order != null ? _mapper.ToDto(order) : null;
        }

        public async Task<ServiceOrderDto> AddAsync(ServiceOrderDto orderDto)
        {
            var order = _mapper.ToEntity(orderDto);
            _context.ServiceOrders.Add(order);
            await _context.SaveChangesAsync();
            
            // Reload the order with all its relationships
            var savedOrder = await GetByIdAsync(order.Id);
            return savedOrder!;
        }

        public async Task UpdateAsync(ServiceOrderDto orderDto)
        {
            var order = _mapper.ToEntity(orderDto);
            _context.ServiceOrders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _context.ServiceOrders
                .Include(o => o.JobActivities)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order != null)
            {
                // Odłącz czynności od zlecenia
                if (order.JobActivities != null)
                {
                    foreach (var job in order.JobActivities)
                    {
                        job.ServiceOrderId = null;
                        job.ServiceOrder = null;
                    }
                }

                // Usuń zlecenie
                _context.ServiceOrders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ServiceOrders.AnyAsync(o => o.Id == id);
        }
    }
}