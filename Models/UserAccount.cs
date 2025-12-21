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
        public string Username { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }
    }
}
