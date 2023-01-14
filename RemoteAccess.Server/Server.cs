using System.Net;
using System.Net.Sockets;

namespace RemoteAccess.Server;

internal static class Server
{
    private static TcpListener _listener = null!;

    public static void Start(int port)
    {
        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Start();
        _listener.BeginAcceptTcpClient(ConnectCallback, null);
    }

    public static void Ping(Client client)
    {
        var ping = new Packet(Packet.Type.Ping);

        client.Send(ping);
    }

    public static void Execute(Client client, string value)
    {
        var packet = new Packet(Packet.Type.None, value);

        client.Send(packet);
    }

    private static void ConnectCallback(IAsyncResult result)
    {
        var client = _listener.EndAcceptTcpClient(result);
        _listener.BeginAcceptTcpClient(ConnectCallback, null);

        Globals.Clients.Add(new Client().Connect(client));

        if (Globals.VerboseLogging)
            Globals.Logs.Add($"({DateTime.Now}) Incoming Connection from {client.Client.RemoteEndPoint}.");
    }
}