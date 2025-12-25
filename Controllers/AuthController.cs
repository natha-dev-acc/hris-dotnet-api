using System;
using System.Linq;
using HRIS_API.Data;
using HRIS_API.DTOs;
using HRIS_API.Models;
using HRIS_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS_API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;
        private readonly EmailService _emailService;

        public AuthController(
            AppDbContext context,
            JwtService jwtService,
            EmailService emailService
        )
        {
            _context = context;
            _jwtService = jwtService;
            _emailService = emailService;
        }

        // 1. REGISTER (TENANT + ADMIN + OTP)
        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            if (_context.UserAccounts.Any(x => x.Email == request.Email))
                return BadRequest("Email already registered");

            var otp = new Random().Next(100000, 999999).ToString();

            // CREATE TENANT
            var tenant = new Tenant
            {
                Name = request.TenantName,
                CreatedAt = DateTime.Now
            };
            _context.Tenants.Add(tenant);
            _context.SaveChanges();

            // GET ADMIN ROLE
            var adminRole = _context.Roles.First(x => x.Name == "admin");

            var user = new UserAccount
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                TenantId = tenant.Id,
                RoleId = adminRole.Id,
                IsActive = false,
                CreatedAt = DateTime.Now,
                OtpCode = otp,
                OtpType = "REGISTER",
                OtpExpiredAt = DateTime.Now.AddMinutes(5)
            };

            _context.UserAccounts.Add(user);
            _context.SaveChanges();

            // SEND OTP EMAIL
            _emailService.SendOtp(
                user.Email,
                "HRIS Account Activation",
                otp
            );

            return StatusCode(201, new
            {
                status = "success",
                data = new
                {
                    tenant_id = tenant.Id,
                    user_id = user.Id,
                    role = "admin"
                }
            });
        }

        // 2. ACTIVATE OTP
        [HttpPost("activate-otp")]
        public IActionResult ActivateOtp(ActivateOtpRequest request)
        {
            var user = _context.UserAccounts.FirstOrDefault(x =>
                x.OtpCode == request.Otp &&
                x.OtpType == "REGISTER" &&
                x.OtpExpiredAt > DateTime.Now
            );

            if (user == null)
                return BadRequest("Invalid or expired OTP");

            user.IsActive = true;
            user.OtpCode = null;
            user.OtpType = null;
            user.OtpExpiredAt = null;

            _context.SaveChanges();

            return Ok("Account activated");
        }

        // 3. RESEND OTP
        [HttpPost("resend-otp")]
        public IActionResult ResendOtp(ResendOtpRequest request)
        {
            var user = _context.UserAccounts.FirstOrDefault(x => x.Email == request.Email);
            if (user == null)
                return NotFound("User not found");

            var otp = new Random().Next(100000, 999999).ToString();

            user.OtpCode = otp;
            user.OtpType = "REGISTER";
            user.OtpExpiredAt = DateTime.Now.AddMinutes(5);

            _context.SaveChanges();

            _emailService.SendOtp(
                user.Email,
                "HRIS Resend OTP",
                otp
            );

            return Ok("OTP resent");
        }

        // 4. LOGIN
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

        // 5. REFRESH TOKEN
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken(RefreshTokenRequest request)
        {
            var token = _context.RefreshTokens.FirstOrDefault(x =>
                x.Token == request.RefreshToken &&
                !x.IsRevoked &&
                x.ExpiredAt > DateTime.Now
            );

            if (token == null)
                return Unauthorized("Invalid refresh token");

            var user = _context.UserAccounts.Find(token.UserId);
            var newAccessToken = _jwtService.GenerateAccessToken(user);

            return Ok(new
            {
                access_token = newAccessToken,
                refresh_token = request.RefreshToken
            });
        }

        // 6. LOGOUT

        /*
        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok("Logout success (stateless)");
        }
        */

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // ambil userId dari JWT
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == "uid");
            if (userIdClaim == null)
                return Unauthorized("Invalid token");

            var userId = int.Parse(userIdClaim.Value);

            // revoke semua refresh token user ini
            var tokens = _context.RefreshTokens
                .Where(x => x.UserId == userId && !x.IsRevoked)
                .ToList();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
            }

            _context.SaveChanges();

            return Ok(new
            {
                status = "success",
                message = "Logout successful"
            });
        }

        // 7. FORGOT PASSWORD OTP
        [HttpPost("forgot-password-otp")]
        public IActionResult ForgotPasswordOtp(ForgotPasswordOtpRequest request)
        {
            var user = _context.UserAccounts.FirstOrDefault(x => x.Email == request.Email);
            if (user == null)
                return NotFound("Email not found");

            var otp = new Random().Next(100000, 999999).ToString();

            user.OtpCode = otp;
            user.OtpType = "RESET_PASSWORD";
            user.OtpExpiredAt = DateTime.Now.AddMinutes(5);

            _context.SaveChanges();

            _emailService.SendOtp(
                user.Email,
                "HRIS Reset Password OTP",
                otp
            );

            return Ok("OTP sent");
        }

        // 8. RESET PASSWORD OTP
        [HttpPost("reset-password-otp")]
        public IActionResult ResetPasswordOtp(ResetPasswordOtpRequest request)
        {
            var user = _context.UserAccounts.FirstOrDefault(x =>
                x.Email == request.Email &&
                x.OtpCode == request.Otp &&
                x.OtpType == "RESET_PASSWORD" &&
                x.OtpExpiredAt > DateTime.Now
            );

            if (user == null)
                return BadRequest("Invalid OTP");

            user.Password = request.Password;
            user.OtpCode = null;
            user.OtpType = null;
            user.OtpExpiredAt = null;

            _context.SaveChanges();

            return Ok("Password updated");
        }
    }
}
