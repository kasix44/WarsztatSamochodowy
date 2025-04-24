using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Numer telefonu")]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        [Display(Name = "Adres e-mail")]
        public string Email { get; set; }

        [StringLength(100)]
        [Display(Name = "Adres zamieszkania")]
        public string Address { get; set; }

        [Display(Name = "Pojazdy")]
        public List<Vehicle> Vehicles { get; set; } = new();
    }
}