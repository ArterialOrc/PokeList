using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using PokeAPI.Services;
using PokeAPI.Services.EmailService;
using PokeAPI.Services.FightStatisticService;
using PokeAPI.Services.PokeAPI;
using PokeAPI.Services.UserService;

var builder = WebApplication.CreateBuilder(args);
string connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<PokeDbContext>(options => options.UseNpgsql(connection));
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IFightStatisticService, FightStatisticService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPokeApi, PokeApi>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddAuthentication("Cookies").AddCookie(options => options.LoginPath = "/login");
builder.Services.AddAuthorization();
builder.Services.AddMemoryCache();
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
