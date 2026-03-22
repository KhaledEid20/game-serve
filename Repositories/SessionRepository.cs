
using game_server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


public class SessionRepository : IBase<Session>, ISession
{
    private readonly AppDbContext _context ;
    public SessionRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<ResultDTO<string>> createSession(Session nSession)
    {
        Session newSession = new Session()
        {
            PlayerId = nSession.PlayerId,
            createdAt = DateTime.UtcNow
        };
        try
        {
            await _context.Sessions.AddAsync(newSession);
            await _context.SaveChangesAsync();
            return new ResultDTO<string>()
            {
                Success = true,
                data = "Session created successfully"
            };
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<ResultDTO<List<Session>>> getAllSession()
    {
        try
        {
            var sessions = await _context.Sessions.ToListAsync();
            return new ResultDTO<List<Session>> { Success = true, data = sessions };
        }
        catch 
        {
            return new ResultDTO<List<Session>> { Success = false, data = null };
        }
    }
}