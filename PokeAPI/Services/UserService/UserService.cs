using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokeAPI.Models;

namespace PokeAPI.Services.UserService
{
    public class UserService : IUserService
    {
        private PokeDbContext _pokeDb;

        public UserService(PokeDbContext pokeDb)
        {
            _pokeDb = pokeDb;
        }

        public async Task AddUser(User user)
        {
            await _pokeDb.Users.AddAsync(user);
            await _pokeDb.SaveChangesAsync();
        }

        public async Task<List<User>> GetUsers()
        {
            return await _pokeDb.Users.ToListAsync();
        }
    }
}