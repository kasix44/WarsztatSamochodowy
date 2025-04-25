using System.Collections.Generic;
using System.Threading.Tasks;
using WorkshopManager.DTOs;

namespace WorkshopManager.Services.Interfaces
{
    public interface IServiceOrderCommentService
    {
        Task<List<ServiceOrderCommentDto>> GetByOrderIdAsync(int serviceOrderId);
        Task<ServiceOrderCommentDto?> GetByIdAsync(int id);
        Task<ServiceOrderCommentDto> AddAsync(ServiceOrderCommentDto commentDto);
        Task UpdateAsync(ServiceOrderCommentDto commentDto);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}