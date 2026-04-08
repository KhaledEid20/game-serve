using System.Reflection;
using System.Threading.Channels;
using game_server.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UDP_Server.Networking;
using UDP_Server.Networking.Packets;
using UDP_Server.Packet_Parsing;
using UDP_Server.Sessions;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<UDPOption>(builder.Configuration.GetSection("UDP"));

builder.Services.AddHttpContextAccessor();


builder.Services.AddSingleton<SendPacket>();
builder.Services.AddSingleton<SessionManagement>();

// THe Channel for UDP Communication
builder.Services.AddSingleton<Channel<RawPacket>>(
    Channel.CreateUnbounded<RawPacket>()
);
builder.Services.AddSingleton<Channel<UpdatedData>>(
    Channel.CreateUnbounded<UpdatedData>()
);

// The Dependency Injection for UDP Client Background Services
builder.Services.AddHostedService<UDPClientListner>();
builder.Services.AddHostedService<PacketParsing>();

// The Dependency Injections for Repositories and Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPlayer, PlayerReposirory>();
builder.Services.AddScoped<ISession, SessionRepository>();

// Serilog configuration
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));


builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}
var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ClientInfo>();

app.MapControllers();

app.Run();
