using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProbaApi.Data;
using ProbaApi.Models;
using System.Threading.Tasks;
using System.Linq;

namespace ProbaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _db;
        public TasksController(AppDbContext db) => _db = db;

        // GET: api/tasks
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _db.Tasks
                .Include(t => t.AssignedUser)
                .Select(t => new
                {
                    id = t.Id,
                    naziv = t.Title,
                    lokacija = t.Status,
                    opis = t.Opis,
                    korisnik = t.AssignedUser != null ? new
                    {
                        ime = t.AssignedUser.Ime,
                        prezime = t.AssignedUser.Prezime,
                        username = t.AssignedUser.Username
                    } : null
                })
                .ToListAsync();

            return Ok(tasks);
        }

        // POST: api/tasks
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _db.Users.FindAsync(dto.AssignedUserId);
            if (user == null)
                return BadRequest(new { error = "Assigned user not found" });

            var task = new TaskItem
            {
                Title = dto.Title,
                Status = dto.Status,
                Opis = dto.Opis,
                AssignedUserId = dto.AssignedUserId
            };

            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                id = task.Id,
                naziv = task.Title,
                opis = task.Opis,
                lokacija = task.Status,
                korisnik = new
                {
                    ime = user.Ime,
                    prezime = user.Prezime,
                    username = user.Username
                }
            });
        }

        // PUT: api/tasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateTaskDto dto)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null)
                return NotFound(new { error = "Zadatak nije pronađen" });

            var user = await _db.Users.FindAsync(dto.AssignedUserId);
            if (user == null)
                return BadRequest(new { error = "Assigned user not found" });

            task.Title = dto.Title;
            task.Status = dto.Status;
            task.Opis = dto.Opis;
            task.AssignedUserId = dto.AssignedUserId;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                id = task.Id,
                naziv = task.Title,
                lokacija = task.Status,
                opis = task.Opis,
                korisnik = new
                {
                    ime = user.Ime,
                    prezime = user.Prezime,
                    username = user.Username
                }
            });
        }

        // DELETE: api/tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _db.Tasks.Include(t => t.AssignedUser).FirstOrDefaultAsync(t => t.Id == id);
            if (task == null)
                return NotFound(new { error = "Zadatak nije pronađen" });

            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                id = task.Id,
                naziv = task.Title,
                lokacija = task.Status,
                opis = task.Opis,
                korisnik = task.AssignedUser != null ? new
                {
                    ime = task.AssignedUser.Ime,
                    prezime = task.AssignedUser.Prezime,
                    username = task.AssignedUser.Username
                } : null
            });
        }
    }
}
