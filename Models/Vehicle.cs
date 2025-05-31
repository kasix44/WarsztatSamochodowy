using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkshopManager.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Marka")]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Required]
        [StringLength(17)]
        public string VIN { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        [Display(Name = "Numer rejestracyjny")]
        public string LicensePlate { get; set; } = string.Empty;

        [Display(Name = "Zdjęcie")]
        public string? ImagePath { get; set; } // Ścieżka do zdjęcia

        // Relacja z klientem
        [Required]
        [Display(Name = "Klient")]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        [Display(Name = "Klient")]
        public Customer? Customer { get; set; }

        // Relacja z zleceniami serwisowymi
        public ICollection<ServiceOrder>? ServiceOrders { get; set; }
    }
}