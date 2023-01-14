namespace RemoteAccess.Server;

// Uploads the server's address to GitHub.
internal static class Address
{
    public static async Task Update()
    {
        try
        {
            var client = new HttpClient();

            var responseMessage = await client.GetAsync("https://ifconfig.me/ip");
            responseMessage.EnsureSuccessStatusCode();
            var address = await responseMessage.Content.ReadAsStringAsync();

            var requestMessage =
                new HttpRequestMessage(HttpMethod.Patch, $"https://api.github.com/gists/{Globals.GistId}");
            requestMessage.Headers.Add("Accept", "application/vnd.github+json");
            requestMessage.Headers.Add("Authorization", $"Bearer {Globals.Token}");
            requestMessage.Headers.Add("User-Agent", "Request");
            requestMessage.Content = new StringContent($"{{\"files\":{{\"Address\":{{\"content\": \"{address}\"}}}}}}");
            responseMessage = await client.SendAsync(requestMessage);
            responseMessage.EnsureSuccessStatusCode();

            Globals.Logs.Add($"({DateTime.Now}) Address Updated to {address}.");
        }
        catch
        {
            Globals.Logs.Add($"({DateTime.Now}) Unable to Update Address.");
        }
    }
}