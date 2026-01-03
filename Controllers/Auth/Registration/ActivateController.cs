using System;
using System.Linq;
using HRIS_API.Data;
using HRIS_API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HRIS_API.Controllers.Auth.Registration
{
    [ApiController]
    [Route("api/auth/registration")]
    public class ActivateController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActivateController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("activate")]
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
    }
}
