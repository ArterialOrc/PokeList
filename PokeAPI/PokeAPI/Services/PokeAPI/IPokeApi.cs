using PokeAPI.Models;

namespace PokeAPI.Services.PokeAPI;

public interface IPokeApi
{
    Task<Pokemon> GetPokeInf(int id);
    Task<List<Pokemon>> GetPokeNames();
    Task<int> GetPokeRandom();
    Pokemon PokemonAttack(Pokemon pokemon, int attackPower);
}