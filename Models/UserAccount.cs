using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRIS_API.Models
{
    [Table("user_account")]
    public class UserAccount
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("username")]
        public string Username { get; set; } = null!;

        [Column("email")]
        public string? Email { get; set; }   // NULLABLE

        [Column("password")]
        public string Password { get; set; } = null!;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("otp_code")]
        public string? OtpCode { get; set; } // NULLABLE

        [Column("otp_expired_at")]
        public DateTime? OtpExpiredAt { get; set; }

        [Column("otp_type")]
        public string? OtpType { get; set; } // NULLABLE

        [Column("last_login")]
        public DateTime? LastLogin { get; set; }

        [Column("tenant_id")]
        public int? TenantId { get; set; }

        [Column("role_id")]
        public int? RoleId { get; set; }
    }
}
