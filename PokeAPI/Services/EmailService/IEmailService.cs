using Microsoft.AspNetCore.Mvc;
using PokeAPI.Models;

namespace PokeAPI.Services.EmailService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string winPoke, string losPoke, string email,string pass);
        Task SendMessage(string email, string message, string subject, string pass);
    }
}
