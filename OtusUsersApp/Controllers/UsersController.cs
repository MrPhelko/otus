using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtusUsersApp.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OtusUsersApp.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UsersContext _usersContext;
        private readonly IJwtUtils _jwtUtils;
        public UsersController(UsersContext context, IJwtUtils jwtUtils)
        {
            _usersContext = context;
            _jwtUtils = jwtUtils;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _usersContext.Users.Select(user => new UserDto { Id = user.Id, Email = user.Email, FullName = user.FullName, Login = user.Login }).ToListAsync());
        }

        [HttpGet("{login}")]
        public async Task<IActionResult> GetById(string login)
        {
            var u = HttpContext.Items["User"] as User;

            if (u == null || u.Login != login)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            var user = await _usersContext.Users.FirstOrDefaultAsync(x => x.Login == login);

            if (user == null)
                return NotFound();

            return Ok(new UserDto { Id = user.Id, Email = user.Email, FullName = user.FullName, Login = user.Login});
        }

        // POST api/<ValuesController>
        //[HttpPost]
        //public async Task<IActionResult> Post([FromBody] User value)
        //{
        //    await _usersContext.AddAsync(value);
        //    await _usersContext.SaveChangesAsync();

        //    return Ok();
        //}

        // PUT api/<ValuesController>/5
        [HttpPut("{login}")]
        public async Task<IActionResult> Put(string login, [FromBody] UserLoginDto value)
        {
            var u = HttpContext.Items["User"] as User;

            if (u == null || u.Login != login)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            var user = await _usersContext
                .Users
                .AsTracking()
                .FirstOrDefaultAsync(x => x.Login == login);

            if (user == null)
                return NotFound();

            user.FullName = value.FullName;
            user.Email = value.Email;

            await _usersContext.SaveChangesAsync();

            return Ok();
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{login}")]
        public async Task<IActionResult> Delete(string login)
        {
            var user = await _usersContext.Users.AsTracking().FirstOrDefaultAsync(x => x.Login == login);

            if (user == null)
                return NotFound();

            _usersContext.Users.Remove(user);

            await _usersContext.SaveChangesAsync();

            return Ok();
        }
    }
}
