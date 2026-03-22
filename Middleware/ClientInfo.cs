using System.Net;
using Azure.Core;
using Microsoft.AspNetCore.Http.Features;

public class ClientInfo
{
    private readonly ILogger<ClientInfo> _logger;
    private readonly RequestDelegate _next;
    public ClientInfo(RequestDelegate next , ILogger<ClientInfo> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        
        var clientIp = context.Connection.RemoteIpAddress;
        var clientport = context.Connection.RemotePort;

        var obj = new
        {
            Ip = clientIp.ToString(),
            port = clientport.ToString()
        };

        _logger.LogInformation("Message Revieved From {@Socket}", obj);
        IPEndPoint remoteEndPoint = null;


        if(clientIp != null)
        {
            remoteEndPoint = new IPEndPoint(clientIp, clientport);
        }
        else
        {
            _logger.LogError("Error on Recieving The Request");
            return;
        }

        context.Items["ClientIP"] = remoteEndPoint;
        await _next(context);
    }
}