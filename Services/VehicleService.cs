using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.DTOs;
using WorkshopManager.Mappers;
using WorkshopManager.Models;
using WorkshopManager.Services.Interfaces;

namespace WorkshopManager.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly ApplicationDbContext _context;
        private readonly VehicleMapper _mapper;

        public VehicleService(ApplicationDbContext context)
        {
            _context = context;
            _mapper = new VehicleMapper();
        }

        public async Task<List<VehicleDto>> GetAllAsync()
        {
            var vehicles = await _context.Vehicles.Include(v => v.Customer).ToListAsync();
            return vehicles.Select(v => _mapper.ToDto(v)).ToList();
        }

        public async Task<VehicleDto?> GetByIdAsync(int id)
        {
            var vehicle = await _context.Vehicles.Include(v => v.Customer).FirstOrDefaultAsync(v => v.Id == id);
            return vehicle != null ? _mapper.ToDto(vehicle) : null;
        }

        public async Task AddAsync(VehicleDto vehicleDto)
        {
            var vehicle = _mapper.ToEntity(vehicleDto);
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(VehicleDto vehicleDto)
        {
            var vehicle = _mapper.ToEntity(vehicleDto);
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
            }
        }

        public bool Exists(int id)
        {
            return _context.Vehicles.Any(v => v.Id == id);
        }
    }
}