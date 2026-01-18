using System.Net.WebSockets;

namespace UDP_Server.Networking.Packets;
public class RawPacket
{
    public MessageType _type { get; set; }
    public int playerId { get; set; }
    public string roomId { get; set; }
    public byte[] payload { get; set; }

}