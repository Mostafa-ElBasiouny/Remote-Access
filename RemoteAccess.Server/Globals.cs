namespace RemoteAccess.Server;

public static class Globals
{
    public static bool ResponseReceived = false;

    // Private GitHub information.
    public const string GistId = "#";
    public const string Token = "#";

    public const int Port = 60_000;
    public static bool VerboseLogging = false;

    public static readonly List<string> Logs = new();
    public static readonly List<Client> Clients = new();
}