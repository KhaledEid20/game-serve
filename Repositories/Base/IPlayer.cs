public interface IPlayer
{
    public Task<ResultDTO<string>> addPlayer(Player newPlayer);
    public Task<ResultDTO<string>> SignInPlayer(string playerId);
    public Task<ResultDTO<List<Player>>> GetAllUser();
    public Task<ResultDTO<string>> JoinRoom();
    
}