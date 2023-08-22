using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using OtusUsersApp.DB;
using System.Security.Claims;
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace OtusUsersApp.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UsersContext _usersContext;
        private readonly IJwtUtils _jwtUtils;
        public AuthController(UsersContext context, IJwtUtils jwtUtils)
        {
            _usersContext = context;
            _jwtUtils = jwtUtils;
        }

        [HttpPost("register")] 
        public async Task<IActionResult> RegisterAsync([FromBody] UserLoginDto user)
        {
            if (user != null)
            {
                var entity = new User
                {
                    FullName = user.FullName,
                    Login = user.Login,
                    Email = user.Email,
                    PasswordHash = HashPassword(user.Password)
                };

                await _usersContext.AddAsync(entity);
                await _usersContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(string login, string password)
        {
            User user = await _usersContext.Users.FirstOrDefaultAsync(x => x.Login == login);
            if (user != null && user.PasswordHash == HashPassword(password))
            {
                var token = _jwtUtils.GenerateToken(user);
                return Ok(new { token, userId = user.Id });
            }
            return BadRequest(new { message = "invalid username or password" });
        }

        private async Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            User user = await _usersContext.Users.FirstOrDefaultAsync(x => x.Login == username);
            if (user != null && user.PasswordHash == HashPassword(password))
            {
                _jwtUtils.GenerateToken(user);
            }

            // если пользователя не найдено
            return null;
        }

        private string HashPassword(string password)
        {
            using var md5 = MD5.Create();
            var md5data = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Encoding.UTF8.GetString(md5data);
        }
    }
}
