using Microsoft.AspNetCore.Mvc;
using ProbaApi.Data;
using ProbaApi.Models;
using System.Linq;

namespace ProbaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AuthController(AppDbContext db) => _db = db;

        // LOGIN
        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Pogrešan email ili šifra");

            return Ok(new
            {
                username = user.Username,
                role = user.Role,
                ime = user.Ime,
                prezime = user.Prezime
            });
        }

        // SIGNUP
        [HttpPost("signup")]
        public IActionResult Signup(SignupDto dto)
        {
            if (_db.Users.Any(u => u.Username == dto.Username))
                return BadRequest("Korisnik već postoji");

            var user = new User
            {
                Ime = dto.Ime,
                Prezime = dto.Prezime,
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "radnik"
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return Ok(new
            {
                message = "Uspešna registracija",
                username = user.Username,
                role = user.Role,
                ime = user.Ime,
                prezime = user.Prezime
            });
        }

        // SET ROLE
        [HttpPost("set-role")]
        public IActionResult SetRole(SetRoleDto dto)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == dto.Username);
            if (user == null)
                return NotFound("Korisnik nije pronađen");

            user.Role = dto.NewRole;
            _db.SaveChanges();

            return Ok(new
            {
                message = "Rola ažurirana",
                username = user.Username,
                role = user.Role,
                ime = user.Ime,
                prezime = user.Prezime
            });
        }

        // DTOs
        public record LoginDto(string Username, string Password);
        public record SignupDto(string Ime, string Prezime, string Username, string Password);
        public record SetRoleDto(string Username, string NewRole);
    }
}
