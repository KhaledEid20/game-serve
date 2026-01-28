using System.Net;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using UDP_Server.Networking.Packets;

namespace UDP_Server.Sessions;
public class SessionManagement
{
    private Dictionary<IPEndPoint , int> EndPoints = new Dictionary<IPEndPoint, int>();
    private Dictionary<int , ClientSession> Sessions = new Dictionary<int, ClientSession>();

    public async Task<int> AddSession(IPEndPoint iP)
    {
        EndPoints[iP] = await GeneratePlayerID();
        Console.WriteLine($"New Session Added : {iP} with ID : {EndPoints[iP]}");
        return EndPoints[iP];
    }

    public async Task<bool> playerIDLookUp(IPEndPoint iP , int playerID)
    {
        if(EndPoints.ContainsKey(iP))
        {
            return EndPoints[iP] == playerID;
        }
        else
        {
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
        return true;
    }

    public bool ConnectionEstablished(int PlayerId)
    {
        if(Sessions[PlayerId].IsConnected == false)
        {
            Sessions[PlayerId].IsConnected = false;
            return true;
        }
        return false;

    }
    private  Task<int> GeneratePlayerID()
    {
        {
            var id = System.Math.Abs(System.Guid.NewGuid().GetHashCode());
            if (id == 0) id = 1;
            return Task.FromResult(id);
        }
    }
}