using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtusUsersApp.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OtusUsersApp.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly UsersContext _usersContext;
        public UsersController(UsersContext context)
        {
            _usersContext = context;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _usersContext.Users.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var user = await _usersContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(user);
        }

        // POST api/<ValuesController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User value)
        {
            await _usersContext.AddAsync(value);
            await _usersContext.SaveChangesAsync();

            return Ok();
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] User value)
        {
            var user = await _usersContext
                .Users
                .AsTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return NotFound();

            user.FullName = value.FullName;
            user.Email = value.Email;
            user.Login = value.Login;

            await _usersContext.SaveChangesAsync();

            return Ok();
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var user = await _usersContext.Users.AsTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return NotFound();

            _usersContext.Users.Remove(user);

            await _usersContext.SaveChangesAsync();

            return Ok();
        }
    }
}
