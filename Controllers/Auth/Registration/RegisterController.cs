using System;
using System.Linq;
using HRIS_API.Data;
using HRIS_API.DTOs;
using HRIS_API.Models;
using HRIS_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRIS_API.Controllers.Auth.Registration
{
    [ApiController]
    [Route("api/auth/registration")]
    public class RegisterController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public RegisterController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            if (_context.UserAccounts.Any(x => x.Email == request.Email))
                return BadRequest("Email already registered");

            var otp = new Random().Next(100000, 999999).ToString();

            var tenant = new Tenant
            {
                Name = request.TenantName,
                CreatedAt = DateTime.Now
            };
            _context.Tenants.Add(tenant);
            _context.SaveChanges();

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

            _emailService.SendOtp(user.Email, "HRIS Account Activation", otp);

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
    }
}
