using System.Net;
using Azure.Core;
using Microsoft.AspNetCore.Http.Features;

public class ClientInfo
{
    private readonly RequestDelegate _next;
    public ClientInfo(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress;
        var clientport = context.Connection.RemotePort;
        IPEndPoint remoteEndPoint = null;

        if(clientIp != null)
        {
            remoteEndPoint = new IPEndPoint(clientIp, clientport);
        }

        context.Items["ClientIP"] = remoteEndPoint;
        await _next(context);
    }
}