using System.Diagnostics.CodeAnalysis;
using game_server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

public class PlayerReposirory : IBase<Player>, IPlayer
{
    private readonly AppDbContext _context;
    public PlayerReposirory(AppDbContext context)
    {
        _context = context;
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
}