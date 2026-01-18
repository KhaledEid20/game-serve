public class Session
{
    public string id { get; set; } =  Guid.NewGuid().ToString();
    public string PlayerId { get; set; }
    public DateTime createdAt { get; set; }
}