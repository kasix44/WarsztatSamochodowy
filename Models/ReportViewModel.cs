using WorkshopManager.Models;

namespace WorkshopManager.Models
{
    public class ReportViewModel
    {
        public List<ServiceOrder> Orders { get; set; } = new();
        public int TotalOrders { get; set; }
        public int TotalParts { get; set; }
        public int TotalActivities { get; set; }
        public decimal TotalCost { get; set; }
    }
} 