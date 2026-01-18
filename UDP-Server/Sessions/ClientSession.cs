using System.Net;


namespace UDP_Server.Sessions;

public class ClientSession
{
    public int clientID { get; set; }
    public IPEndPoint ClientIP { get; set; }
    public bool IsConnected { get; set; }
    public int? RoomId { get; set; }
}