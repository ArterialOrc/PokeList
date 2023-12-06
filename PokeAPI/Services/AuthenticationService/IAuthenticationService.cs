namespace PokeAPI.Services.AuthenticationService
{
    public interface IAuthenticationService
    {
        bool Registration(string email, string password);
        bool Authorization(string email, string password);
        void ChangePassword(string email, string newPass);
    }
}
