using Microsoft.AspNetCore.Mvc;
using PokeAPI.Models;

namespace PokeAPI.Services.UserService
{
    public class UserService : IUserService
    {
        private PokeDbContext _pokeDb;

        public UserService([FromServices] PokeDbContext pokeDb)
        {
            this._pokeDb = pokeDb;
        }

        public User? GetUser(string email)
        {
            return _pokeDb.Users.FirstOrDefault(user => user.Email.Equals(email));
        }

        public bool IsUserExists(string email)
        {
            var user = _pokeDb.Users.FirstOrDefault(u => u.Email.Equals(email));
            return user != null;
        }

        public void AddUser(string email, string password, byte[] salt)
        {
            var user = new User() 
            { 
                Email = email, 
                Password = password, 
                Salt = Convert.ToBase64String(salt)
            };
            _pokeDb.Users.Add(user);
            _pokeDb.SaveChanges();
        }

        public void ChangePassword(string email, string newPassword)
        {
            var user = GetUser(email);
            user.Password = newPassword;
            _pokeDb.SaveChanges();
        }
    }
}