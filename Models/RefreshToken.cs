using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRIS_API.Models
{
    [Table("refresh_token")]
    public class RefreshToken
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("token")]
        public string Token { get; set; }

        [Column("expired_at")]
        public DateTime ExpiredAt { get; set; }

        [Column("is_revoked")]
        public bool IsRevoked { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
