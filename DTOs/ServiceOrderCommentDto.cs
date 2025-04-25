using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.DTOs;

public class ServiceOrderCommentDto
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Treść")]
    public string Content { get; set; } = string.Empty;

    [Display(Name = "Data dodania")]
    public DateTime CreatedAt { get; set; }

    public string? AuthorId { get; set; }
    public string? AuthorUserName { get; set; }

    [Required]
    public int ServiceOrderId { get; set; }
    public ServiceOrderDto? ServiceOrder { get; set; }
} 