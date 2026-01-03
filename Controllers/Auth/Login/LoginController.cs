using System;
using System.Linq;
using HRIS_API.Data;
using HRIS_API.DTOs;
using HRIS_API.Models;
using HRIS_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRIS_API.Controllers.Auth.Login
{
    [ApiController]
    [Route("api/auth")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public LoginController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            var user = _context.UserAccounts.FirstOrDefault(x =>
                x.Username == request.Username &&
                x.Password == request.Password &&
                x.IsActive
            );

            if (user == null)
                return Unauthorized("Invalid credentials");

            user.LastLogin = DateTime.Now;

            var accessToken = _jwtService.GenerateAccessToken(user);

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
                ExpiredAt = DateTime.Now.AddDays(7),
                CreatedAt = DateTime.Now
            };

            _context.RefreshTokens.Add(refreshToken);
            _context.SaveChanges();

            return Ok(new
            {
                access_token = accessToken,
                refresh_token = refreshToken.Token,
                token_type = "Bearer"
            });
        }
    }
}
