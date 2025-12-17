public class Player
{
    public string id { get; set; } = new Guid().ToString();
    public string playerName { get; set; }
    public DateTime joinedAt { get; set; }
}