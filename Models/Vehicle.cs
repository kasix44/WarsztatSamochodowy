using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkshopManager.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Brand { get; set; }

        [Required]
        [StringLength(50)]
        public string Model { get; set; }

        [Required]
        [StringLength(17)]
        public string VIN { get; set; }

        [Required]
        [StringLength(10)]
        public string LicensePlate { get; set; }

        public string? ImagePath { get; set; } // Ścieżka do zdjęcia

        // Relacja z klientem
        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }
    }
}