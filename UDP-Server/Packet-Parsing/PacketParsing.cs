using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using UDP_Server.Networking.Packets;

namespace UDP_Server.Packet_Parsing;
public class PacketParsing : BackgroundService
{
    private readonly Channel<RawPacket> _channel;
    public PacketParsing(Channel<RawPacket> channel)
    {
        _channel = channel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var packet = await _channel.Reader.ReadAsync(stoppingToken);
                Console.WriteLine("Packet Received from Channel: " + packet._type.ToString());
                
                // if(packet._type == MessageType.JoinSuccess)
                // {
                    
                // }
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

    }
}