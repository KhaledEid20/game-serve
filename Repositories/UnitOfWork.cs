using game_server.Data;

public class UnitOfWork : IUnitOfWork
{
    public IPlayer Players { get; set ; }
    public ISession Sessions { get; set ; }
    public AppDbContext _context { get; set; }

    public UnitOfWork(IPlayer playerRepository, ISession sessionRepository , AppDbContext context)
    {
        Players = playerRepository;
        Sessions = sessionRepository;
        _context = context;
    }
    public void Dispose()
    {
    }
}