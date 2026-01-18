
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Options;
using UDP_Server.Networking.Packets;

namespace UDP_Server.Networking;

class UDPClientListner : BackgroundService
{
    private readonly Channel<RawPacket> _channel;
    private readonly UDPOption _options;
    public UDPClientListner(Channel<RawPacket> channel, IOptions<UDPOption> options)
    {
        _channel = channel;
        _options = options.Value;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using UdpClient listner = new UdpClient(_options.IpAdress , _options.Port);
        Console.WriteLine($"UDP Client Listner Started on {_options.IpAdress}/{_options.Port}...");
        while (!stoppingToken.IsCancellationRequested)
        {
            UdpReceiveResult result;
            try
            {
                result = await listner.ReceiveAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }   
            byte[] buffer = result.Buffer;
            string json = Encoding.UTF8.GetString(buffer);

            RawPacket packet = JsonSerializer.Deserialize<RawPacket>(json);

            await _channel.Writer.WriteAsync(packet, stoppingToken);
        }
    }   
}
