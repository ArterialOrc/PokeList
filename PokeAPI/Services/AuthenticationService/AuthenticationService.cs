using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using PokeAPI.Models;
using PokeAPI.Services.UserService;

namespace PokeAPI.Services.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {
        private IUserService userService;

        public AuthenticationService(IUserService userService)
        {
            this.userService = userService;
        }

        public bool Registration(string email, string password)
        {
            if (userService.IsUserExists(email))
                return false;
            var salt = RandomNumberGenerator.GetBytes(16);
            var hashPassword = CreateHashPassword(password, salt);
            userService.AddUser(email, hashPassword, salt);
            return true;
        }

        public bool Authorization(string email, string password)
        {
            if (!userService.IsUserExists(email))
                return false;
            var user = userService.GetUser(email);
            return CheckPassword(password, user);
        }

        public void ChangePassword(string email, string newPass)
        {
            var user = userService.GetUser(email);
            var salt = Convert.FromBase64String(user.Salt);
            var hashPassword = CreateHashPassword(newPass, salt);
            userService.ChangePassword(email, hashPassword);
        }

        private bool CheckPassword(string password, User user)
        {
            var salt = Convert.FromBase64String(user.Salt);
            var hashedInputPassword = CreateHashPassword(password, salt);
            return user.Password.Equals(hashedInputPassword);
        }

        private string CreateHashPassword(string password, byte[] salt)
        {
            var hashedPassword = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 32));
            return hashedPassword;
        }
    }
}
