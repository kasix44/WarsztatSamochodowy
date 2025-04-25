using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.DTOs;
using WorkshopManager.Mappers;
using WorkshopManager.Models;
using WorkshopManager.Services.Interfaces;

namespace WorkshopManager.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;
        private readonly CustomerMapper _mapper;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
            _mapper = new CustomerMapper();
        }

        public async Task<List<CustomerDto>> GetAllAsync(string? search = null)
        {
            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    c.FirstName.Contains(search) || c.LastName.Contains(search));
            }

            var customers = await query.ToListAsync();
            return customers.Select(c => _mapper.ToDto(c)).ToList();
        }

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Vehicles)
                .FirstOrDefaultAsync(c => c.Id == id);
                
            return customer != null ? _mapper.ToDto(customer) : null;
        }

        public async Task AddAsync(CustomerDto customerDto)
        {
            var customer = _mapper.ToEntity(customerDto);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CustomerDto customerDto)
        {
            var customer = _mapper.ToEntity(customerDto);
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Customers.AnyAsync(c => c.Id == id);
        }
    }
}