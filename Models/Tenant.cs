using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRIS_API.Models
{
    [Table("tenant")]
    public class Tenant
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
