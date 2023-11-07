using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokeAPI.Models;

namespace PokeAPI.Services.FightStatisticService
{
    public class FightStatisticService : IFightStatisticService
    {
        private PokeDbContext _pokeDb;

        public FightStatisticService(PokeDbContext pokeDb)
        {
            _pokeDb = pokeDb;
        }

        public async Task AddFightStatistic(FightStatistic statistic)
        {
            await _pokeDb.Statistics.AddAsync(statistic);
            await _pokeDb.SaveChangesAsync();
        }

        public async Task<List<FightStatistic>> GetFightStatistics()
        {
            return await _pokeDb.Statistics.ToListAsync();
        }
    }
}
