using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using PokeAPI.Models;
using PokeAPI.Services.FightStatisticService;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using PokeAPI.Services;
using PokeAPI.Services.EmailService;
using PokeAPI.Services.PokeAPI;
using PokeAPI.Services.UserService;
using AuthenticationService = PokeAPI.Services.AuthenticationService.AuthenticationService;
using IAuthenticationService = PokeAPI.Services.AuthenticationService.IAuthenticationService;

namespace PokeAPI.Controllers
{
    record Error(string Message);
    public class HomeController : Controller
    {
        private readonly IFileProvider _fileProvider;
        private IDistributedCache cache;
        
        private List<Pokemon> pokes = new List<Pokemon>();

        public HomeController(IFileProvider fileProvider,IDistributedCache cache)
        {
            _fileProvider = fileProvider;
            this.cache = cache;
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
            var currentUser = service.GetUser(HttpContext.User.Identity.Name);
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
        public Task<IActionResult> PokemonAttack([FromServices] IPokeApi pokeApi,int myNum,[FromBody] EnemiesPokes lst)
        {
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
                return Task.FromResult<IActionResult>(Json(new List<Pokemon>() {lst.myPoke,lst.enemyPoke}));
            }

            return Task.FromResult<IActionResult>(Json(null));
        }
        
        [HttpPost("pokemon/email")]
        public async Task SendEmail([FromServices] IEmailService service)
        {
            var request = HttpContext.Request;
            var body = await request.ReadFromJsonAsync<Email>();
            string pass = Environment.GetEnvironmentVariable("YASMTP_PASSWORD");
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
        public async Task SetStatistics([FromServices] IFightStatisticService service,[FromServices] IUserService userService)
        {
            var request = HttpContext.Request;
            var statistic = await request.ReadFromJsonAsync<FightStatistic>();
            if(statistic != null)
            {
                statistic.Id = Guid.NewGuid().ToString();
                if (User.Identity.Name != null) statistic.UserId = userService.GetUser(User.Identity.Name)!.UserId;
                await service.AddFightStatistic(statistic);
            }
        }

        [HttpGet("/login")]
        public IActionResult Log()
        {
            var filePath = _fileProvider.GetFileInfo("/html/login.html").PhysicalPath;
            var contentType = "text/html";
            return PhysicalFile(filePath, contentType);
        }

        [HttpPost("/pass-change")]
        public async Task<IActionResult> Pass([FromBody] User user, [FromServices] IAuthenticationService authenticationService,[FromServices] IEmailService emailer)
        {
            var newPass = new Random().Next(10_000, 100_000);
            authenticationService.ChangePassword(user.Email, newPass.ToString());
            string pass = Environment.GetEnvironmentVariable("YASMTP_PASSWORD");
            await emailer.SendMessage(user.Email, $"Новый пароль: {newPass}", "Новый пароль для PokeAPI", pass);
            return Ok();
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] User user,[FromServices] IAuthenticationService authenticationService,[FromServices] IEmailService emailer)
        {
            if(!authenticationService.Authorization(user.Email, user.Password))
                return BadRequest(new { Message = "Неправильный логин или пароль" });

            var code = new Random().Next(1000, 10000);
            await HashCode(user.Email, code);
            string pass = Environment.GetEnvironmentVariable("YASMTP_PASSWORD");
            await emailer.SendMessage(user.Email, $"Код: {code}", "Код для авторизации на PokeAPI",pass);
            return Ok();
        }

        [HttpPost("/code")]
        public async Task<IActionResult> Code([FromForm] string email, [FromForm] string code)
        {
            var origCode = await cache.GetStringAsync(email);
            if (!origCode.Equals(code))
                return BadRequest();
            cache.Remove(email);
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, email) };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return Redirect("/");
        }

        [NonAction]
        private async Task HashCode(string email, int code)
        {
            await cache.SetStringAsync(email, $"{code}", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(4)
            });
        }
        
        
        [HttpGet("/registration")]
        public IActionResult Page()
        {
            var filePath = _fileProvider.GetFileInfo("/html/registration.html").PhysicalPath;
            var contentType = "text/html";
            return PhysicalFile(filePath, contentType);
        }

        [HttpPost("/registration")]
        public IActionResult Registration([FromServices] IAuthenticationService service,[FromForm] string email, [FromForm] string password)
        {
            if (!service.Registration(email, password))
                return Conflict(new { Message = "Пользователь с таким email уже существует" });
            return RedirectPermanent("/");
        }
        
        [HttpPost("/yandex-login")]
        public async Task<IActionResult> YandexLogin([FromServices] IAuthenticationService authenticationService, [FromBody] User user)
        {
            authenticationService.Registration(user.Email, "123");
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Email) };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return Redirect("/");
        }

        [HttpGet("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Вы вышли из системы");
        }

        [HttpGet("/current")]
        public IActionResult Current()
        {
            return Content($"Current user: {User.Identity.Name}");
        }

        [HttpGet("/yandex-data")]
        public IActionResult YandexData([FromServices] YandexApi yandexApi)
        {
            return Ok(new
            {
                ClientId = yandexApi.ClientId,
                ResponseType = yandexApi.ResponseType,
                RedirectUri = yandexApi.RedirectUri,
                TokenPageOrigin = yandexApi.TokenPageOrigin
            });
        }

        [HttpGet("/yandex-test")]
        public IActionResult YandexTest()
        {
            var filePath = _fileProvider.GetFileInfo("/html/test.html").PhysicalPath;
            var contentType = "text/html";
            return PhysicalFile(filePath, contentType);
        }

    }
}