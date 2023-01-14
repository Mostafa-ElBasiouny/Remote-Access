using RemoteAccess.Server;
using System.Collections;
using System.Drawing;
using System.Net.Sockets;
using System.Text;

namespace RemoteAccess.Server;

public class Client
{
    public TcpClient Socket = null!;

    private const int BufferSize = 9000000;
    private byte[] _buffer = null!;
    private Packet _packet = null!;
    private NetworkStream _networkStream = null!;

    public Client Connect(TcpClient socket)
    {
        Socket = socket;
        Socket.ReceiveBufferSize = BufferSize;
        Socket.SendBufferSize = BufferSize;

        _buffer = new byte[BufferSize];
        _packet = new Packet();
        _networkStream = Socket.GetStream();
        _networkStream.BeginRead(_buffer, 0, BufferSize, ReceiveCallback, null);

        Server.Ping(this);

        return this;
    }

    public void Disconnect()
    {
        Globals.Clients.Remove(this);
        Globals.Logs.Add($"({DateTime.Now}) Disconnected Client {Socket.Client.RemoteEndPoint}.");

        Socket.Close();
    }

    public void Send(Packet packet)
    {
        try
        {
            _networkStream.BeginWrite(packet.Pack(), 0, packet.Count(), null, null);

            if (Globals.VerboseLogging)
                Globals.Logs.Add($"({DateTime.Now}) Sent Packet to Client {Socket.Client.RemoteEndPoint}.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error sending packet {e}");
            Globals.Logs.Add($"({DateTime.Now}) Unable to Send Packet to Client {Socket.Client.RemoteEndPoint}.");
        }
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            var bytesReadLength = _networkStream.EndRead(result);

            if (bytesReadLength <= 0)
            {
                Disconnect();
                return;
            }

            var bytesReadBuffer = new byte[bytesReadLength];
            Array.Copy(_buffer, bytesReadBuffer, bytesReadLength);

            Handle(bytesReadBuffer);

            _networkStream.BeginRead(_buffer, 0, BufferSize, ReceiveCallback, null);
        }
        catch
        {
            Disconnect();
        }
    }

    private void Handle(byte[] buffer)
    {
        var type = _packet.Unpack(buffer, out var value, out var extension);

        switch (type)
        {
            case Packet.Type.None:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(value);
                Console.ResetColor();
                Globals.ResponseReceived = true;
                break;
            case Packet.Type.Ping:
                Globals.Logs.Add($"({DateTime.Now}) Established Connection with {Socket.Client.RemoteEndPoint}.");
                break;
            default:
                Globals.Logs.Add($"({DateTime.Now}) Received Unknown Packet from {Socket.Client.RemoteEndPoint}.");
                throw new Exception();
        }
    }
}