using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WorkshopManager.Models
{
    public class AddUsedPartViewModel
    {
        public int ServiceOrderId { get; set; }

        [Required]
        public int PartId { get; set; }

        [Required]
        [Range(1, 999)]
        public int Quantity { get; set; }

        public SelectList? Parts { get; set; }
    }

}