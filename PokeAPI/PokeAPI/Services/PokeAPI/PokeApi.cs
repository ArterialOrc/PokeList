using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using PokeAPI.Models;

namespace PokeAPI.Services.PokeAPI
{
    public class PokeApi : IPokeApi
    {
        private const string _API_URL = "https://pokeapi.co/api/v2/pokemon";

        private async Task<int> GetPokeCount()
        {
            int count = 0;
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(_API_URL);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var pokeData = JObject.Parse(content);
                    count = int.Parse((string)pokeData["count"]);
                }
            }
            return count;
        }

        public async Task<List<Pokemon>> GetPokeNames()
        {
            int limit = await GetPokeCount();
            var pokemons = new List<Pokemon>();

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{_API_URL}?limit={limit}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var pokeData = JObject.Parse(content);
                    var reg = new Regex(@"/[0-9]+/");
                    foreach (var pokeName in pokeData["results"])
                    {
                        pokemons.Add(new Pokemon()
                        {
                            Id = int.Parse(reg.Match(pokeName["url"].ToString()).Value.Replace("/","")),
                            Name = (string)pokeName["name"],
                        });
                    }
                }
            }
            return pokemons;
        }

        public async Task<Pokemon> GetPokeInf(int id)
        {
            Pokemon pokemon = null!;

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{_API_URL}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var pokeData = JObject.Parse(content);
                    pokemon = new Pokemon()
                    {
                        Id = int.Parse((string)pokeData["id"]),
                        Name = (string)pokeData["name"],
                        Hp = int.Parse((string)pokeData["stats"][0]["base_stat"]),
                        AttackPower = int.Parse((string)pokeData["stats"][1]["base_stat"]),
                        Height = int.Parse((string)pokeData["height"]),
                        Weight = int.Parse((string)pokeData["weight"]),
                        Image = (string?)pokeData["sprites"]["other"]["official-artwork"]["front_default"]
                    };
                }
            }
            return pokemon;
        }
        public async Task<int> GetPokeRandom()
        {
            Pokemon pokemon = null!;
            var lst = await GetPokeNames();
            Random rnd = new Random();
            
            return lst[rnd.Next(lst.Count)].Id;
        }
        public Pokemon PokemonAttack(Pokemon pokemon, int attackPower)
        {
            pokemon.Hp = Math.Max(0, pokemon.Hp - attackPower);
            return pokemon;
        }

    }
}
