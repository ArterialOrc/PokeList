using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;


namespace PokeAPI.Models;
[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public string Id { get; set; }

    [NotNull]
    [Column("email")]
    public string Email { get; set; }

    [NotNull]
    [Column("password")]
    public string Password { get; set; }
    
}