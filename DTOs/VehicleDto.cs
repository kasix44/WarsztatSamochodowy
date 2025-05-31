using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.DTOs;

public class VehicleDto
{
    public int Id { get; set; }
    
    [Required]
    [Display(Name = "Marka")]
    public string Brand { get; set; } = string.Empty;
    
    [Required]
    [Display(Name = "Model")]
    public string Model { get; set; } = string.Empty;
    
    [Required]
    [Display(Name = "Numer rejestracyjny")]
    public string LicensePlate { get; set; } = string.Empty;
    
    [Display(Name = "VIN")]
    public string VIN { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Klient")]
    public int CustomerId { get; set; }

    [Display(Name = "Zdjęcie")]
    public string? ImagePath { get; set; }

    // Navigation property
    public CustomerDto? Customer { get; set; }
} 