using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace PokeAPI.Models
{
    [Table("fight_statistic")]
    public class FightStatistic
    {
        [Key]
        [Column("id")]
        public string Id { get; set; }

        [NotNull]
        [Column("win_poke")]
        public string WinPoke { get; set; }

        [NotNull]
        [Column("los_poke")]
        public string LosPoke { get; set; }
        
        [Column("user_id")]
        public Guid UserId { get; set; }
    }
}
