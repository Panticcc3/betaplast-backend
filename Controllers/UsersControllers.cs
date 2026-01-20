using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProbaApi.Data;
using System.Linq;

namespace ProbaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        // Vraća sve korisnike (za direktor.html)
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users
                .Select(u => new
                {
                    id = u.Id,
                    ime = u.Ime,
                    prezime = u.Prezime,
                    username = u.Username,
                    role = u.Role
                })
                .ToList();

            return Ok(users);
        }

        // GET: api/users/me?username=...
        // Vraća podatke o trenutno ulogovanom korisniku
        [HttpGet("me")]
        public IActionResult GetMe([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest(new { error = "Username je obavezan parametar" });

            var user = _context.Users
                .Include(u => u.Tasks)
                .FirstOrDefault(u => u.Username == username);

            if (user == null)
                return NotFound(new { error = "Korisnik nije pronađen" });

            var zadaci = user.Tasks
                .Select(t => new
                {
                    naziv = t.Title,
                    opis = t.Opis,
                    lokacija = t.Status
                })
                .ToList();

            return Ok(new
            {
                username = user.Username,
                ime = user.Ime,
                prezime = user.Prezime,
                status = "Aktivan",
                pozicija = user.Role,
                kontakt = user.Username,
                zadaci
            });
        }
    }
}
