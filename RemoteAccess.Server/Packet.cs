using System.Text;

namespace RemoteAccess.Server;

public class Packet
{
    public enum Type
    {
        None,
        Ping
    }

    private readonly List<byte>? _buffer;

    public Packet() => _buffer = new List<byte>();

    public Packet(Type type, string? value = null, string? extension = null)
    {
        _buffer = new List<byte> { Convert.ToByte(type) };

        switch (type)
        {
            case Type.None:
                _buffer.AddRange(BitConverter.GetBytes(Convert.ToInt32(value?.Length ?? 0)));
                _buffer.AddRange(Encoding.ASCII.GetBytes(value!));
                break;
            case Type.Ping:
                return;
            default:
                throw new Exception();
        }
    }

    public byte[] Pack()
    {
        using var memoryStream = new MemoryStream();
        using var binaryWriter = new BinaryWriter(memoryStream, Encoding.ASCII);

        binaryWriter.Write(_buffer!.ToArray());

        return memoryStream.ToArray();
    }

    public Type Unpack(byte[] buffer, out string? value, out string? extension)
    {
        using var memoryStream = new MemoryStream(buffer);
        using var binaryReader = new BinaryReader(memoryStream, Encoding.ASCII);

        var type = (Type)binaryReader.ReadByte();

        switch (type)
        {
            case Type.None:
                var count = binaryReader.ReadUInt32();
                value = Encoding.ASCII.GetString(binaryReader.ReadBytes((int)count));
                extension = string.Empty;
                break;
            case Type.Ping:
                value = string.Empty;
                extension = string.Empty;
                break;
            default:
                throw new Exception();
        }

        return type;
    }

    public int Count() => _buffer!.Count;
}