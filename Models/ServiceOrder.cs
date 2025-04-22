using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WorkshopManager.Models
{
    public enum OrderStatus
    {
        Nowe,
        WTrakcie,
        Zakończone
    }

    public class ServiceOrder
    {
        public int Id { get; set; }

        [Required]
        public DateTime StartDate { get; set; } = DateTime.Now;

        public DateTime? EndDate { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        // Powiązanie z pojazdem
        [Required]
        public int VehicleId { get; set; }

        [ValidateNever]
        public Vehicle? Vehicle { get; set; }

        // Przypisany mechanik (opcjonalnie)
        public string? AssignedMechanicId { get; set; }

        [ForeignKey("AssignedMechanicId")]
        [ValidateNever]
        public IdentityUser? AssignedMechanic { get; set; }

        // TODO: dodamy później: lista czynności, części, komentarzy
    }
}