using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Text.RegularExpressions;
using PokeAPI.Models;

namespace PokeAPI.Services.EmailService;

public class EmailService : IEmailService
{
    public async Task SendEmailAsync(string winPoke,string losPoke,string email,string pass)
    {
        MailAddress from = new MailAddress("cevamaltsev@yandex.ru", "Maltsev");
        MailAddress to = new MailAddress(email);
        MailMessage m = new MailMessage(from, to);
        m.Subject = "Результаты боя";
        m.Body = $"Loose Pokemon ={losPoke};\n Win Pokemon ={winPoke}";
        SmtpClient smtp = new SmtpClient("smtp.yandex.ru", 587);
        smtp.Credentials = new NetworkCredential("cevamaltsev@yandex.ru",pass);
        smtp.EnableSsl = true;
        await smtp.SendMailAsync(m);
        Console.WriteLine("Письмо отправлено");
    }

    public async Task SendMessage(string email, string message,string subject, string pass)
    {
        MailAddress from = new MailAddress("cevamaltsev@yandex.ru", "Maltsev");
        MailAddress to = new MailAddress(email);
        MailMessage m = new MailMessage(from, to);
        m.Subject = subject;
        m.Body = message;
        SmtpClient smtp = new SmtpClient("smtp.yandex.ru", 587);
        smtp.Credentials = new NetworkCredential("cevamaltsev@yandex.ru",pass);
        smtp.EnableSsl = true;
        await smtp.SendMailAsync(m);
        Console.WriteLine("Письмо отправлено");
    }
}