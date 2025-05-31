using Microsoft.AspNetCore.Mvc.Rendering;

namespace WorkshopManager.Models
{
    public class ReportFilterViewModel
    {
        public List<SelectListItem> Customers { get; set; } = new();
        public List<SelectListItem> LicensePlates { get; set; } = new();
        public string? CustomerEmail { get; set; }
        public string? LicensePlate { get; set; }
        public string? Month { get; set; }
    }
}