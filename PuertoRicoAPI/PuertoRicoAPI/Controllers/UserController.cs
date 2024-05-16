using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuertoRicoAPI.Data;

namespace PuertoRicoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        public UserController(DataContext context) 
        { 
            _context = context;
        }

        [HttpGet]
        public  async Task<ActionResult<List<User>>> GetUsers()
        {
            return Ok(await _context.Users.ToListAsync());
        }

        [HttpPost]
         public async Task<ActionResult<List<User>>> CreateUser(User user)
        {
            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(await _context.Users.ToListAsync());
        }


        [HttpPut]

        public async Task<ActionResult<List<User>>> UpdateUser(User user)
        {
            var dbUser = await _context.Users.FindAsync(user.Id);
            if(dbUser == null)
            {
                return BadRequest("User not found.");
            }   

            dbUser.Username = user.Username;
            dbUser.Password = user.Password;

            await _context.SaveChangesAsync();

            return Ok(await _context.Users.ToListAsync());
        }

        [HttpDelete("{id}")]
        
        public async Task<ActionResult<List<User>>> DeleteUser(int id)
        {
            var dbUser = await _context.Users.FindAsync(id);
            if (dbUser == null)
                return BadRequest("User not found.");

            _context.Users.Remove(dbUser);
            await _context.SaveChangesAsync();

            return Ok(await _context.Users.ToListAsync());
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<User>> AuthenticateUser(User user)
        {
            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username && u.Password == user.Password);

            if (dbUser == null)
            {
                return NotFound();
            }

            return dbUser;
        }
    }
}
