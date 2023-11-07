using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Linq;
using PokeAPI.Models;
using PokeAPI.Services.FightStatisticService;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using System.Text;
using FluentFTP;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using PokeAPI.Services.EmailService;
using PokeAPI.Services.PokeAPI;
using PokeAPI.Services.UserService;

namespace PokeAPI.Controllers
{
    record Error(string Message);
    public class HomeController : Controller
    {
        private readonly IFileProvider _fileProvider;
        
        private List<Pokemon> pokes = new List<Pokemon>();

        public HomeController(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        [HttpGet("/")]
        [HttpGet("/index")]
        public IActionResult Index()
        {
            var filePath = _fileProvider.GetFileInfo("/html/index.html").PhysicalPath;
            var contentType = "text/html";
            return PhysicalFile(filePath, contentType);
        }
        
        [HttpGet("pokemon/list")]
        public async Task<IActionResult> Index([FromServices] IPokeApi pokeApi)
        {
            var pokes = await pokeApi.GetPokeNames();
            return Json(pokes);
        }

        [HttpGet("pokemon/{id}")]
        public async Task<IActionResult> PokemonWithId(int id,[FromServices] IPokeApi pokeApi)
        {
            var pokemon = await pokeApi.GetPokeInf(id);
            return Json(pokemon);
        }
        
        [Authorize]
        [HttpPost("pokemon/save/{id}")]
        public async Task<IActionResult> PokemonSave(int id,[FromServices] IPokeApi pokeApi,[FromServices] IUserService service)
        {
            var pokemon = await pokeApi.GetPokeInf(id);
            var currentUser =( await service.GetUsers()).FirstOrDefault(p => (p.Email == HttpContext.User.Identity.Name));
            string username = currentUser.Email;
            string password = currentUser.Password;
            
            string markdownFile = $"# Name: {pokemon.Name}\n" +
                                  $"Id: {pokemon.Id}\n" +
                                  $"Hp: {pokemon.Hp}\n" +
                                  $"AttackPower: {pokemon.AttackPower}\n" +
                                  $"Height: {pokemon.Height}\n" +
                                  $"Weight: {pokemon.Weight}\n" +
                                  $"Image({pokemon.Image})";
            string folder = DateTime.Now.ToString("yyyyMMdd");
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://127.0.0.1/{username}/{folder}");
            request.Credentials = new NetworkCredential(username, password);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            WebResponse response = await request.GetResponseAsync();
            response.Close();
            
            request = (FtpWebRequest)WebRequest.Create($"ftp://127.0.0.1/{username}/{folder}/{pokemon.Name}.md");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(username, password);
            byte[] file = Encoding.UTF8.GetBytes(markdownFile);
            request.ContentLength = file.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                await requestStream.WriteAsync(file, 0, file.Length);
            }
            
            return Json(pokemon);
        }
        
        [HttpGet("pokemon/random")]
        public async Task<IActionResult> PokemonRandom([FromServices] IPokeApi pokeApi)
        {
            var pokemonId = await pokeApi.GetPokeRandom();
            return Json(pokemonId);
        }
        
        [HttpGet("pokemon/fight")]
        public async Task<IActionResult> PokemonFight([FromServices] IPokeApi pokeApi, [FromQuery] Enemies enemies)
        {
            var myPoke = await pokeApi.GetPokeInf(enemies.myPoke);
            var enemyPoke = await pokeApi.GetPokeInf(enemies.enemyPoke);
            var lst = new List<Pokemon>() { myPoke, enemyPoke };
            return Json(lst);
        }

        [HttpPost("pokemon/fight/{myNum}")]
        public async Task<IActionResult> PokemonAttack([FromServices] IPokeApi pokeApi,int myNum)
        {
            var request = HttpContext.Request;
            var lst = await request.ReadFromJsonAsync<EnemiesPokes>();
            if(lst != null)
            {
                var rnd = new Random().Next(10);

                if ((myNum + rnd) % 2 == 0)
                {
                    pokeApi.PokemonAttack(lst.myPoke,lst.enemyPoke.AttackPower);
                }
                else
                {
                    pokeApi.PokemonAttack(lst.enemyPoke,lst.myPoke.AttackPower);
                }
                return Json(new List<Pokemon>() {lst.myPoke,lst.enemyPoke});
            }

            return Json(null);
        }
        
        [HttpPost("pokemon/email")]
        public async Task SendEmail([FromServices] IEmailService service)
        {
            var request = HttpContext.Request;
            var body = await request.ReadFromJsonAsync<Email>();
            string pass = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("YandexSMTP")["Password"];
            if(body != null)
            {
                await service.SendEmailAsync(body.statistic.winPoke,body.statistic.losPoke, body.email,pass);
            }
        }

        [HttpGet("/info")]
        public IActionResult Info()
        {
            var filePath = _fileProvider.GetFileInfo("/html/info.html").PhysicalPath;
            var contentType = "text/html";
            return PhysicalFile(filePath, contentType);
        }

        [HttpGet("/fight")]
        public IActionResult Fight()
        {
            var filePath = _fileProvider.GetFileInfo("/html/fight.html").PhysicalPath;
            var contentType = "text/html";
            return PhysicalFile(filePath, contentType);
        }

        [HttpGet("api/stat")]
        public async Task<IActionResult> GetStatistics([FromServices] IFightStatisticService service)
        {
            var statistics = await service.GetFightStatistics();
            return Json(statistics);
        }

        [HttpPost("api/stat")]
        public async Task SetStatistics([FromServices] IFightStatisticService service)
        {
            var request = HttpContext.Request;
            var statistic = await request.ReadFromJsonAsync<FightStatistic>();
            if(statistic != null)
            {
                statistic.Id = Guid.NewGuid().ToString();
                await service.AddFightStatistic(statistic);
            }
        }

        [HttpGet("/login")]
        public IActionResult Login()
        {
            var filePath = _fileProvider.GetFileInfo("/html/login.html").PhysicalPath;
            var contentType = "text/html";
            return PhysicalFile(filePath, contentType);
        }

        [HttpPost("/login")]
        public async Task<IResult> LoginPost([FromServices] IUserService service)
        {
            
            var form = HttpContext.Request.Form;
            
            if (!form.ContainsKey("email") || !form.ContainsKey("password"))
                return Results.BadRequest("Email и/или пароль не установлены");
 
            string email = form["email"];
            string password = form["password"];

            var users = await service.GetUsers();
            // находим пользователя 
            User? user = users.FirstOrDefault(p => p.Email == email && p.Password == password);
            // если пользователь не найден, отправляем статусный код 401
            if (user is null) return Results.Unauthorized();
 
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Email) };
            // создаем объект ClaimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return Results.Redirect("/");;
        }
        
        [HttpGet("/logout")]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Results.Redirect("/login");
        }

    }
}