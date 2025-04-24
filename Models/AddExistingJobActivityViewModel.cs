using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkshopManager.Models;

namespace WorkshopManager.Models
{
    public class AddExistingJobActivityViewModel
    {
        public int ServiceOrderId { get; set; }

        [Display(Name = "Czynność")]
        public int SelectedJobActivityId { get; set; }

        public List<JobActivity>? AvailableJobActivities { get; set; }
    }
}