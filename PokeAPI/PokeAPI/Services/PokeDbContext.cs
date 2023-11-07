using Microsoft.EntityFrameworkCore;
using PokeAPI.Models;

namespace PokeAPI.Services
{
    public class PokeDbContext : DbContext
    {
        public DbSet<FightStatistic> Statistics { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        public PokeDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
