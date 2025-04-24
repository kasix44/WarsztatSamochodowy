using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkshopManager.Models
{
    public class ServiceOrderComment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Treść komentarza")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Data dodania")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("ServiceOrder")]
        public int ServiceOrderId { get; set; }
        public ServiceOrder? ServiceOrder { get; set; }

        [Display(Name = "Autor")]
        public string? Author { get; set; }
        
        [Display(Name = "ID autora")]
        public string? AuthorId { get; set; }
    }
}