using System;
using System.Linq;
using HRIS_API.Data;
using HRIS_API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HRIS_API.Controllers.Auth.PasswordRecovery
{
    [ApiController]
    [Route("api/auth/password-recovery")]
    public class ResetPasswordController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ResetPasswordController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("reset")]
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
