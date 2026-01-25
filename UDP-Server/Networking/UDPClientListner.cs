
using System.Net;
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
        var ip = IPAddress.Parse(_options.IpAdress);
        using UdpClient listner = new UdpClient(new IPEndPoint(ip, _options.Port));
        while (!stoppingToken.IsCancellationRequested)
        {
            UdpReceiveResult result;
            try
            {
                result = await listner.ReceiveAsync(stoppingToken);

                Console.WriteLine("Message received from " + result.RemoteEndPoint.ToString());

                byte[] messageReceived = Encoding.ASCII.GetBytes("Message Received\n");
                await listner.SendAsync(messageReceived, messageReceived.Length, result.RemoteEndPoint);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            byte[] buffer = result.Buffer;
            string json = Encoding.UTF8.GetString(buffer);

            var packet = JsonSerializer.Deserialize<RawPacket>(json);


            try
            {
                await _channel.Writer.WriteAsync(packet, stoppingToken);
                Console.WriteLine("Packet Sent to Channel: " + packet._type.ToString());
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation Canceled while writing to channel");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deserializing packet: " + ex.Message);
            }
            
        }
    }
}
