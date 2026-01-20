public interface IUnitOfWork : IDisposable
{
    IPlayer Players { get; set; }
    ISession Sessions { get; set; }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}