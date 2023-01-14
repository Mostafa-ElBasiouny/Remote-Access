using System.Net;

namespace RemoteAccess.Client;

public static class Globals
{
    // Private GitHub information.
    public const string Token = "#";
    public const string GistId = "#";

    public const int Port = 60_000;
    public static IPAddress? IpAddress { get; set; }
}