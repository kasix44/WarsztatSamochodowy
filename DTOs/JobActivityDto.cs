using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.DTOs;

public class JobActivityDto
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Opis")]
    public string Description { get; set; } = string.Empty;

    [Range(0, 99999)]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Display(Name = "Koszt robocizny")]
    public decimal LaborCost { get; set; }

    [Display(Name = "Zlecenie (opcjonalne)")]
    public int? ServiceOrderId { get; set; }

    public ServiceOrderDto? ServiceOrder { get; set; }
} 