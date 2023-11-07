using Microsoft.AspNetCore.Mvc;
using PokeAPI.Models;

namespace PokeAPI.Services.FightStatisticService
{
    public interface IFightStatisticService
    {
        Task<List<FightStatistic>> GetFightStatistics();
        Task AddFightStatistic(FightStatistic statistic);
    }
}
