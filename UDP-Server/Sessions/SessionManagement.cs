using System.Net;

namespace UDP_Server.Sessions;
public class SessionManagement
{
    private Dictionary<IPEndPoint , int> EndPoints = new Dictionary<IPEndPoint, int>();
    private Dictionary<int , ClientSession> Sessions = new Dictionary<int, ClientSession>();

    public async Task AddSession(IPEndPoint iP)
    {
        EndPoints[iP] = await GeneratePlayerID();
        Console.WriteLine($"New Session Added : {iP} with ID : {EndPoints[iP]}");
    }

    private async Task<int> GeneratePlayerID()
    {
        {
            var id = System.Math.Abs(System.Guid.NewGuid().GetHashCode());
            if (id == 0) id = 1;
            return id;
        }
    }
}