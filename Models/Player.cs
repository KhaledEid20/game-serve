public class Player
{
    public string id { get; set; } = Guid.NewGuid().ToString();
    public string playerName { get; set; }
    public DateTime joinedAt { get; set; }
}