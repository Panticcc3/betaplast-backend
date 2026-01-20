using System.ComponentModel.DataAnnotations;

namespace ProbaApi.Models
{
    public class CreateTaskDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Status { get; set; }

        [Required]
        public int AssignedUserId { get; set; }

        public string Opis { get; set; } = string.Empty;
    }
}
