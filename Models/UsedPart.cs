using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkshopManager.Models;

public class UsedPart
{
    public int Id { get; set; }

    [Required]
    public int PartId { get; set; }

    [ForeignKey("PartId")]
    public Part Part { get; set; }

    [Required]
    [Range(1, 999)]
    public int Quantity { get; set; }

    [Required]
    public int ServiceOrderId { get; set; }

    [ForeignKey("ServiceOrderId")]
    public ServiceOrder ServiceOrder { get; set; }
}
