using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkshopManager.DTOs;

namespace WorkshopManager.Models
{
    public class AddExistingJobActivityViewModel
    {
        public int ServiceOrderId { get; set; }

        [Display(Name = "Czynność")]
        public int SelectedJobActivityId { get; set; }

        public List<JobActivityDto>? AvailableJobActivities { get; set; }
    }
}