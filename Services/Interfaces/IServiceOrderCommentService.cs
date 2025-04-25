using System.Collections.Generic;
using System.Threading.Tasks;
using WorkshopManager.Models;

namespace WorkshopManager.Services.Interfaces
{
    public interface IServiceOrderCommentService
    {
        Task<List<ServiceOrderComment>> GetByOrderIdAsync(int serviceOrderId);
        Task<ServiceOrderComment?> GetByIdAsync(int id);
        Task AddAsync(ServiceOrderComment comment);
        Task UpdateAsync(ServiceOrderComment comment);
        Task DeleteAsync(int id);
        bool Exists(int id);
    }
}