
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
    private readonly ILogger<UDPClientListner> _logger;
    public UDPClientListner(Channel<RawPacket> channel, IOptions<UDPOption> options, ILogger<UDPClientListner> logger)
    {
        _channel = channel;
        _options = options.Value;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var ip = IPAddress.Parse(_options.IpAdress);
        using UdpClient listner = new UdpClient(new IPEndPoint(ip, _options.Port));
        _logger.LogInformation("The UDP Server Listening on : {ip} / {port}", ip, _options.Port);
        while (!stoppingToken.IsCancellationRequested)
        {
            UdpReceiveResult result;
            try
            {
                result = await listner.ReceiveAsync(stoppingToken);

                _logger.LogInformation("Message Recieved from {@Source}", new { SenderIp = result.RemoteEndPoint.ToString() });

                byte[] messageReceived = Encoding.ASCII.GetBytes("Message Received\n");
                await listner.SendAsync(messageReceived, messageReceived.Length, result.RemoteEndPoint);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Can't receive the Message");
                break;
            }
            try
            {
                byte[] buffer = result.Buffer;
                string json = Encoding.UTF8.GetString(buffer);

                var packet = JsonSerializer.Deserialize<RawPacket>(json);
                packet.clientIP = result.RemoteEndPoint;

                _logger.LogInformation("Packet Serialized Successfully");

                try
                {
                    await _channel.Writer.WriteAsync(packet, stoppingToken);
                    _logger.LogInformation("Data Pushed to The Channel 1 {Packet}", packet);
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogError(ex, "Error while Pushing the Packet Through the Channel 1");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while Pushing the Packet Through the Channel 1");
            }

        }
    }
}
