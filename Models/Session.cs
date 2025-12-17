public class Session
{
    public string id { get; set; } = new Guid().ToString();
    public string PlayerId { get; set; }
    public DateTime createdAt { get; set; }
}