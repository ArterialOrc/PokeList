using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using PokeAPI.Services;
using PokeAPI.Services.EmailService;
using PokeAPI.Services.FightStatisticService;
using PokeAPI.Services.PokeAPI;
using PokeAPI.Services.UserService;


Env.Load("Env/.env.docker");

var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST");
var redisPort = Environment.GetEnvironmentVariable("REDIS_PORT");
var ftpHost = Environment.GetEnvironmentVariable("FTP_HOST");

var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST");
var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT");
var dbName = Environment.GetEnvironmentVariable("POSTGRES_DATABASE");
var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USERNAME");
var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

var builder = WebApplication.CreateBuilder(args);
string connection = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddDbContext<PokeDbContext>(options => options.UseNpgsql($"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}"));
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IFightStatisticService, FightStatisticService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPokeApi, PokeApi>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = $"{redisHost}:{redisPort}";
});
builder.Services.AddAuthentication("Cookies").AddCookie(options => options.LoginPath = "/login");
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IFileProvider>(
        new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));


var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
