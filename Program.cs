using System.Reflection;
using game_server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UDP_Server.Sessions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<UDPOption>(builder.Configuration.GetSection("UDPOption"));

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<SessionManagement>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPlayer, PlayerReposirory>();
builder.Services.AddScoped<ISession, SessionRepository>();


builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}
var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ClientInfo>();

app.MapControllers();

app.Run();