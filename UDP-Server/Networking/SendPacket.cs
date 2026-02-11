using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using UDP_Server.Networking.Packets;

namespace UDP_Server.Networking;

public class SendPacket
{
       private readonly UDPOption _options;
       private readonly ILogger<SendPacket> _logger;
       public SendPacket(IOptions<UDPOption> options , ILogger<SendPacket> logger)
       {
            _options = options.Value;
            _logger = logger;
       }
       
       public async Task Send(RawPacket packet , IPEndPoint remote)
    {
        var Sender = new UdpClient();

        try
        {
            string json = JsonSerializer.Serialize(packet);
            byte[] data = Encoding.UTF8.GetBytes(json);
            await Sender.SendAsync(data , data.Length , remote);
            _logger.LogInformation("Data sent via UDP Succefully : {@Packet}" , packet);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex ,"can't Send the Packet via UDP");
        }
    }
}