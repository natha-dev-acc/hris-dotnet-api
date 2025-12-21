using HRIS_API.Data;
using HRIS_API.DTOs;
using HRIS_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRIS_API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            var user = new UserAccount
            {
                Username = request.Username,
                Password = request.Password,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.UserAccounts.Add(user);
            _context.SaveChanges();

            return Ok("Register success");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            var user = _context.UserAccounts.FirstOrDefault(x =>
                x.Username == request.Username &&
                x.Password == request.Password &&
                x.IsActive == true
            );

            if (user == null)
                return Unauthorized("Invalid username or password");

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
                ExpiredAt = DateTime.Now.AddDays(7),
                IsRevoked = false,
                CreatedAt = DateTime.Now
            };

            _context.RefreshTokens.Add(refreshToken);
            _context.SaveChanges();

            return Ok(new
            {
                message = "Login success",
                refresh_token = refreshToken.Token
            });
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken(RefreshTokenRequest request)
        {
            var token = _context.RefreshTokens
                .FirstOrDefault(x =>
                    x.Token == request.RefreshToken &&
                    !x.IsRevoked &&
                    x.ExpiredAt > DateTime.Now);

            if (token == null)
                return Unauthorized("Invalid refresh token");

            return Ok("Refresh token valid");
        }

        [HttpPost("logout")]
        public IActionResult Logout(RefreshTokenRequest request)
        {
            var token = _context.RefreshTokens
                .FirstOrDefault(x => x.Token == request.RefreshToken);

            if (token == null)
                return NotFound("Token not found");

            token.IsRevoked = true;
            _context.SaveChanges();

            return Ok("Logout success");
        }
    }
}
