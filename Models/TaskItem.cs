using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProbaApi.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Opis { get; set; } = string.Empty;

        public string? Status { get; set; }

        [Required]
        public int AssignedUserId { get; set; }

        // Navigaciona svojina ne sme biti zahtevana u POST telu
        [ValidateNever]
        public User? AssignedUser { get; set; }

        
    }
}
