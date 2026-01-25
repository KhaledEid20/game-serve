using System.Diagnostics.CodeAnalysis;
using System.Net;
using game_server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using UDP_Server.Sessions;

public class PlayerReposirory : IBase<Player>, IPlayer
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private SessionManagement _sessionManagement;
    private readonly UDPOption _options;
    public PlayerReposirory(AppDbContext context, IHttpContextAccessor httpContextAccessor , SessionManagement sessionManagement , IOptions<UDPOption> options)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _sessionManagement = sessionManagement; 
        _options = options.Value;
    }
    public async Task<ResultDTO<string>> addPlayer(Player nPlayer)
    {
        try
        {
            var newplayer = new Player()
            {
                playerName = nPlayer.playerName,
                joinedAt = DateTime.UtcNow
            };
            await _context.Players.AddAsync(newplayer);
            await _context.SaveChangesAsync();
            return new ResultDTO<string>()
            {
                Success = true,
                data = "Player added successfully"
            };
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<ResultDTO<string>> SignInPlayer(string playerId)
    {
        try
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
            {
                return new ResultDTO<string>()
                {
                    Success = false,
                    data = "Player not found"
                };
            }
            player.joinedAt = DateTime.UtcNow;
            _context.Players.Update(player);
            await _context.SaveChangesAsync();
            return new ResultDTO<string>()
            {
                Success = true,
                data = "Player joined successfully"
            };
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<ResultDTO<List<Player>>> GetAllUser()
    {
        try
        {
            var player = await _context.Players.ToListAsync();
            if (player == null)
            {
                return new ResultDTO<List<Player>>()
                {
                    Success = false,
                    data = null
                };
            }
            return new ResultDTO<List<Player>>()
            {
                Success = true,
                data = player
            };
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<ResultDTO<UDPSocket>> JoinRoom()
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
            {
                return await Task.FromResult(new ResultDTO<UDPSocket>()
                {
                    Success = false,
                    data = null
                });
            }

            if (context.Items.TryGetValue("ClientIP", out var clientIpObj) && clientIpObj is IPEndPoint clientIp)
            {

                var playerID = await _sessionManagement.AddSession(clientIp);
                
                return new ResultDTO<UDPSocket>()
                {
                    Success = true,
                    data = new UDPSocket
                    {
                        playerID = playerID,
                        ServerIP = _options.IpAdress,
                        port = _options.Port
                    }
                };
            }
            else
            {
                return await Task.FromResult(new ResultDTO<UDPSocket>()
                {
                    Success = false,
                    data = null
                });
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}