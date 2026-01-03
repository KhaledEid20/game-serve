public interface ISession
{
    public Task<ResultDTO<string>> createSession(Session nSession);
    public Task<ResultDTO<List<Session>>> getAllSession();
}