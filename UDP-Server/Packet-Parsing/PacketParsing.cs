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
    private readonly ILogger<PacketParsing> _logger;
    public PacketParsing(Channel<RawPacket> channel ,
     SessionManagement sessionManagement ,
      SendPacket sendPacket ,
       Channel<UpdatedData>gameloop,
       ILogger<PacketParsing> logger)
       
    {
        _channel = channel;
        _sessionManagement = sessionManagement;
        _sendPacket = sendPacket;
        _gameloopChannel = gameloop;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("The Packet Parsing Service Currently Up");
            try
            {
                var packet = await _channel.Reader.ReadAsync(stoppingToken);
                Console.WriteLine("Packet Received from Channel: " + packet._type.ToString());
                
                if(packet._type == MessageType.JoinRequest)
                {
                    var newPacket = new RawPacket();
                    try 
                    {
                        int playerID = await _sessionManagement.AddSession(packet.clientIP);
                        _logger.LogInformation("The Player added to the Connection Queue player id {playerId} attahed" , playerID);
                        newPacket = new RawPacket()
                        {
                            _type = MessageType.JoinConfirmation,
                            playerId = playerID,
                            roomId = packet.roomId,
                        };
                        await _sessionManagement.AddClientSession(packet); // create the clientSession object
                        await _sendPacket.Send(newPacket , packet.clientIP); // send confirmation
                    }
                    catch
                    {
                        _logger.LogWarning("Player ID mismatch for IP {packet.clientIP}. Cannot join room {packet.roomId}." , packet.clientIP , packet.roomId);

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
                        _logger.LogWarning("Player ID mismatch for IP {packet.clientIP}. Cannot join room {packet.roomId}." , packet.clientIP , packet.roomId);
                    }
                }
                if(packet._type == MessageType.stateUpdate)
                {
                    try
                    {
                        await _gameloopChannel.Writer.WriteAsync(packet.data);
                        _logger.LogInformation("The data is passed to the gameLoop Service");
                    }
                    catch (OperationCanceledException ex)
                    {
                        _logger.LogError(ex , "Data can't be passed Through the gameLoop Channel");
                    }
                    
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex , "The Packet Parser Service is Down");
                return;
            }
        }
    }
}