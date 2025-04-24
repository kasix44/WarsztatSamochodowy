using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.Models
{
    public class Part
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nazwa części")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, 99999)]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Cena jednostkowa")]
        public decimal UnitPrice { get; set; }
    }
}