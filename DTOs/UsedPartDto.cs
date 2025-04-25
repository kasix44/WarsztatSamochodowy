using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.DTOs;

public class UsedPartDto
{
    public int Id { get; set; }

    [Required]
    [Range(1, 999)]
    [Display(Name = "Ilość")]
    public int Quantity { get; set; }

    [Required]
    [Display(Name = "Część")]
    public int PartId { get; set; }

    public PartDto? Part { get; set; }

    [Required]
    [Display(Name = "Zlecenie")]
    public int ServiceOrderId { get; set; }

    public ServiceOrderDto? ServiceOrder { get; set; }
} 