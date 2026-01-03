using System;
using System.Linq;
using HRIS_API.Data;
using HRIS_API.DTOs;
using HRIS_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRIS_API.Controllers.Auth.Registration
{
    [ApiController]
    [Route("api/auth/registration")]
    public class ResendController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public ResendController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("resend")]
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

            _emailService.SendOtp(user.Email, "HRIS Resend OTP", otp);

            return Ok("OTP resent");
        }
    }
}
