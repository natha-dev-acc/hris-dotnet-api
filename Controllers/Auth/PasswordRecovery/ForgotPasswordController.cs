using System;
using System.Linq;
using HRIS_API.Data;
using HRIS_API.DTOs;
using HRIS_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRIS_API.Controllers.Auth.PasswordRecovery
{
    [ApiController]
    [Route("api/auth/password-recovery")]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public ForgotPasswordController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("forgot")]
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

            _emailService.SendOtp(user.Email, "HRIS Reset Password OTP", otp);

            return Ok("OTP sent");
        }
    }
}
