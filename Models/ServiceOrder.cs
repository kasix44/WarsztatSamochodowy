using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WorkshopManager.Models
{
    public enum OrderStatus
    {
        WTrakcie,
        Zakończone
    }

    public class ServiceOrder
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Data rozpoczęcie")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Display(Name = "Data zakończenia")]
        public DateTime? EndDate { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        // Powiązanie z pojazdem
        [Required]
        [Display(Name = "ID Pojazdu")]
        public int VehicleId { get; set; }

        [ValidateNever]
        [Display(Name = "Pojazd")]
        public Vehicle? Vehicle { get; set; }

        public string? AssignedMechanicId { get; set; }

        [ForeignKey("AssignedMechanicId")]
        [ValidateNever]
        public IdentityUser? AssignedMechanic { get; set; }

        // TODO: dodamy później: komentarzy
        public List<UsedPart> UsedParts { get; set; } = new();
        public ICollection<JobActivity>? JobActivities { get; set; }

    }
}