namespace HRIS_API.DTOs
{
    public class ResetPasswordOtpRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Otp { get; set; }
    }
}
