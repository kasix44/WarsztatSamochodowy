using WorkshopManager.DTOs;

namespace WorkshopManager.Services.Interfaces
{
    public interface IVehicleService
    {
        Task<List<VehicleDto>> GetAllAsync();
        Task<VehicleDto?> GetByIdAsync(int id);
        Task AddAsync(VehicleDto vehicleDto);
        Task UpdateAsync(VehicleDto vehicleDto);
        Task DeleteAsync(int id);
        bool Exists(int id);
    }
}