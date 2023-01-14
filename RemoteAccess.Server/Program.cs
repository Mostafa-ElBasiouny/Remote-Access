using RemoteAccess.Server;

// await Address.Update();
Server.Start(Globals.Port);

ConsoleKeyInfo consoleKeyInfo;

void Input()
{
    consoleKeyInfo = Console.ReadKey();
    Console.Clear();
}

void Menu()
{
    Console.Clear();
    Console.Out.Flush();
    Console.CursorVisible = false;

    Console.WriteLine("1. Display Server Status");
    Console.WriteLine("2. Display Server Logs");
    Console.WriteLine("3. Display Connected Clients");

    Console.WriteLine();

    if (Globals.Clients.Count > 0)
    {
        Console.WriteLine("4. Start Remote Shell Session");
        Console.WriteLine();
    }

    Console.WriteLine($"5. {(Globals.VerboseLogging ? "Disable" : "Enable")} Verbose Logging");
    if (Globals.Logs.Count > 0)
        Console.WriteLine("6. Clear Server Logs");

    Console.WriteLine("\n[Press ESC to exit]");

    Input();
}

do
{
    Menu();

    switch (consoleKeyInfo.Key)
    {
        case ConsoleKey.D1:
            Console.WriteLine("Server Status\n");
            Console.WriteLine($"Port Number       ({Globals.Port})");
            Console.WriteLine($"Verbose Logging   ({Globals.VerboseLogging})");
            Console.WriteLine($"Connected Clients ({Globals.Clients.Count})");
            break;
        case ConsoleKey.D2:
            Console.WriteLine($"Server Logs ({Globals.Logs.Count})\n");
            Globals.Logs.ForEach(Console.WriteLine);
            break;
        case ConsoleKey.D3:
            Console.WriteLine($"Connected Clients ({Globals.Clients.Count})\n");
            Globals.Clients.ForEach(client =>
                Console.WriteLine(
                    $"({Globals.Clients.IndexOf(client).ToString("D4")}) {client.Socket.Client.RemoteEndPoint}"));
            break;
        case ConsoleKey.D4:
            if (Globals.Clients.Count == 0) continue;
            Console.WriteLine("Remote Shell Session\n");
            Globals.Clients.ForEach(client =>
                Console.WriteLine(
                    $"({Globals.Clients.IndexOf(client).ToString("D4")}) {client.Socket.Client.RemoteEndPoint}"));

            Console.CursorVisible = true;
            Console.Write("\n(Client) ");
            var client = Globals.Clients.ElementAtOrDefault(Convert.ToInt32(Console.ReadLine()));

            if (client == null) continue;
            Console.Clear();
            do
            {
                Console.Write($"Remote PS {client.Socket.Client.RemoteEndPoint}> ");
                var cmd = Console.ReadLine();

                if (cmd!.Contains("exit")) break;

                Globals.ResponseReceived = false;

                Server.Execute(client, cmd);

                while (!Globals.ResponseReceived)
                {
                }
            } while (true);

            continue;
        case ConsoleKey.D5:
            Globals.VerboseLogging = !Globals.VerboseLogging;
            continue;
        case ConsoleKey.D6:
            Globals.Logs.Clear();
            continue;
        default:
            continue;
    }

    Input();
} while (consoleKeyInfo.Key != ConsoleKey.Escape);