using System.Text;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var _config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.Development.json", optional: false)
        .Build();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"])),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

var app = builder.Build();

app.MapControllers();
app.UseCors(x => x.AllowAnyHeader().AllowAnyHeader().WithOrigins("https://localhost:4200"));
app.UseAuthentication();
app.UseAuthorization();
app.Run();

