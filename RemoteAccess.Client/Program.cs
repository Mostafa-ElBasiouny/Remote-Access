using System.Net;
using RemoteAccess.Client;

// await Address.Update();
Globals.IpAddress = IPAddress.Parse("127.0.0.1");
Client.Connect();

Console.ReadKey();