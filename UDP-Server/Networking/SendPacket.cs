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
       public SendPacket(IOptions<UDPOption> options)
       {
            _options = options.Value;
       }
       
       public async Task Send(RawPacket packet , IPEndPoint remote)
    {
        var Sender = new UdpClient(_options.IpAdress , _options.Port);

        try
        {
            string json = JsonSerializer.Serialize(packet);
            byte[] data = Encoding.UTF8.GetBytes(json);
            await Sender.SendAsync(data , data.Length , remote);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}