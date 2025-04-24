using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkshopManager.Models
{
    public class JobActivity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Opis")]
        public string Description { get; set; } = string.Empty;

        [Range(0, 99999)]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Koszt robocizny")]
        public decimal LaborCost { get; set; }

        [ForeignKey("ServiceOrder")]
        [Display(Name = "Zlecenie (opcjonalne)")]
        public int? ServiceOrderId { get; set; }

        public ServiceOrder? ServiceOrder { get; set; }
    }
}