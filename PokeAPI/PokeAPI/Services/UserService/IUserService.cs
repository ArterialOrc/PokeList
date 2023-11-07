using Microsoft.AspNetCore.Mvc;
using PokeAPI.Models;

namespace PokeAPI.Services.UserService
{
    public interface IUserService
    {
        Task<List<User>> GetUsers();
        Task AddUser(User user);
    }
}