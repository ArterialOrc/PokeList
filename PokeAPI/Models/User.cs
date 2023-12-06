using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PokeAPI.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public Guid UserId { get; private set; } = Guid.NewGuid();

        [Column("email")]
        public string Email { get; set; }

        [Column("password")]
        public string Password { get; set; }
        [Column("salt")]
        public string Salt { get; set; }
    }
}