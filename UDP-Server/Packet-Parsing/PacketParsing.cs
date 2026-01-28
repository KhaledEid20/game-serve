using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using UDP_Server.Networking;
using UDP_Server.Networking.Packets;
using UDP_Server.Sessions;

namespace UDP_Server.Packet_Parsing;
public class PacketParsing : BackgroundService
{
    private readonly Channel<RawPacket> _channel;
    private readonly Channel<UpdatedData> _gameloopChannel;
    private readonly SessionManagement _sessionManagement;
    private readonly SendPacket _sendPacket;
    public PacketParsing(Channel<RawPacket> channel , SessionManagement sessionManagement , SendPacket sendPacket , Channel<UpdatedData>gameloop)
    {
        _channel = channel;
        _sessionManagement = sessionManagement;
        _sendPacket = sendPacket;
        _gameloopChannel = gameloop;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var packet = await _channel.Reader.ReadAsync(stoppingToken);
                Console.WriteLine("Packet Received from Channel: " + packet._type.ToString());
                
                if(packet._type == MessageType.JoinRequest)
                {
                    var newPacket = new RawPacket();
                    if(await _sessionManagement.playerIDLookUp(packet.clientIP , packet.playerId))
                    {
                        newPacket = new RawPacket()
                        {
                            _type = MessageType.JoinConfirmation,
                            playerId = packet.playerId,
                            roomId = packet.roomId,
                        };
                        await _sessionManagement.AddClientSession(packet); // create the clientSession object
                        await _sendPacket.Send(newPacket , packet.clientIP); // send confirmation
                    }
                    else
                    {
                        newPacket = new RawPacket()
                        {
                            _type = MessageType.JoinFailure,
                            playerId = packet.playerId,
                            roomId = packet.roomId,
                            payload = System.Text.Encoding.UTF8.GetBytes("Invalid Player ID")
                        };
                        await _sendPacket.Send(newPacket , packet.clientIP); // send Rejection

                    }   
                }
                if(packet._type == MessageType.JoinSuccess)
                {
                    if(await _sessionManagement.playerIDLookUp(packet.clientIP , packet.playerId))
                    {
                        
                        if(await _sessionManagement.ConnectionLookUp(packet.playerId))
                        {
                            _sessionManagement.ConnectionEstablished(packet.playerId);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Player ID mismatch for IP {packet.clientIP}. Cannot join room {packet.roomId}.");
                    }
                }
                if(packet._type == MessageType.stateUpdate)
                {
                    await _gameloopChannel.Writer.WriteAsync(packet.data);
                    Console.WriteLine("The Updated Data is moved to the gameloop Channel");
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }
}