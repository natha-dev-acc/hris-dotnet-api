using System.Linq;
using HRIS_API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS_API.Controllers.Auth.Login
{
    [ApiController]
    [Authorize]
    [Route("api/auth")]
    public class LogoutController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LogoutController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == "uid");
            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            var tokens = _context.RefreshTokens
                .Where(x => x.UserId == userId && !x.IsRevoked)
                .ToList();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
            }

            _context.SaveChanges();

            return Ok("Logout successful");
        }
    }
}
