using System.Net;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using UDP_Server.Networking.Packets;

namespace UDP_Server.Sessions;
public class SessionManagement
{
    private Dictionary<IPEndPoint , int> EndPoints = new Dictionary<IPEndPoint, int>();
    private Dictionary<int , ClientSession> Sessions = new Dictionary<int, ClientSession>();
    private readonly ILogger<SessionManagement> _logger;
    public SessionManagement(ILogger<SessionManagement> logger)
    {
        _logger = logger;
    }
    public async Task<int> AddSession(IPEndPoint iP)
    {
        EndPoints[iP] = await GeneratePlayerID();
        _logger.LogInformation("New Session Added : {iP} with ID : {EndPoints[iP]}" , iP, EndPoints[iP]);
        return EndPoints[iP];
    }

    public async Task<bool> playerIDLookUp(IPEndPoint iP , int playerID)
    {
        if(EndPoints.ContainsKey(iP))
        {
            _logger.LogInformation("The IP : {IP} is Assigned To The Player ID : {Player Id}", iP , EndPoints[iP]);
            return EndPoints[iP] == playerID;
        }
        else
        {
            _logger.LogError("The Ip isn't Exist in The Connection Queue");
            return false;
        }
    }

    public async Task<bool> ConnectionLookUp(int PlayerId)
    {

        return Sessions.ContainsKey(PlayerId);
    }

    public async Task<bool> AddClientSession(RawPacket packet)
    {
        if(packet.playerId == 0 || Sessions.ContainsKey(packet.playerId))
        {
            _logger.LogWarning("The Session is Alreay Existed");
            return false;
        }
        var session = new ClientSession()
        {
            clientID = packet.playerId,
            ClientIP = packet.clientIP,
            IsConnected = false,
            RoomId = packet.roomId
        };
        Sessions[packet.playerId] = session;
        _logger.LogInformation("The new Session Just added to the Session Queue");
        return true;
    }

    public bool ConnectionEstablished(int PlayerId)
    {
        if(Sessions[PlayerId].IsConnected == false)
        {
            Sessions[PlayerId].IsConnected = true;
            _logger.LogInformation("The Connection Established Succefully");
            return true;
        }
        _logger.LogWarning("The Connection is Already Established");
        return false;
    }
    private Task<int> GeneratePlayerID()
    {
        {
            var id = System.Math.Abs(System.Guid.NewGuid().GetHashCode());
            if (id == 0) id = 1;
            return Task.FromResult(id);
        }
    }
}