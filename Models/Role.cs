using System.ComponentModel.DataAnnotations.Schema;

namespace HRIS_API.Models
{
    [Table("role")]
    public class Role
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}
