using System.ComponentModel.DataAnnotations;
using WorkshopManager.Models;

namespace WorkshopManager.DTOs;

public class ServiceOrderDto
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Data rozpoczęcia")]
    public DateTime StartDate { get; set; }

    [Display(Name = "Data zakończenia")]
    public DateTime? EndDate { get; set; }

    [Required]
    public OrderStatus Status { get; set; }

    [Required]
    [Display(Name = "ID Pojazdu")]
    public int VehicleId { get; set; }

    [Display(Name = "Pojazd")]
    public VehicleDto? Vehicle { get; set; }

    public string? AssignedMechanicId { get; set; }
    public string? AssignedMechanicUserName { get; set; }

    public List<UsedPartDto> UsedParts { get; set; } = new();
    public List<JobActivityDto> JobActivities { get; set; } = new();
    public List<ServiceOrderCommentDto> Comments { get; set; } = new();
} 