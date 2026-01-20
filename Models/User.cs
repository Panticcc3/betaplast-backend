namespace ProbaApi.Models
{
    public class User
    {
        public int Id { get; set; }

        // Podaci o radniku
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;

        // Email se koristi kao username za login
        public string Username { get; set; } = string.Empty;

        // Hashirana šifra
        public string PasswordHash { get; set; } = string.Empty;

        // Rola korisnika (default "radnik")
        public string Role { get; set; } = "radnik";

        // Navigaciona veza ka zadacima
        public List<TaskItem> Tasks { get; set; } = new();
    }
}
