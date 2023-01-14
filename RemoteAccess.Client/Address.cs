using System.Net;
using System.Text.RegularExpressions;

namespace RemoteAccess.Client;

internal static class Address
{
    public static async Task Update()
    {
        var client = new HttpClient();
        
        var requestMessage =
            new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/gists/{Globals.GistId}");

        requestMessage.Headers.Add("Accept", "application/vnd.github+json");
        requestMessage.Headers.Add("Authorization", $"Bearer {Globals.Token}");
        requestMessage.Headers.Add("User-Agent", "Request");

        var responseMessage = await client.SendAsync(requestMessage);
        responseMessage.EnsureSuccessStatusCode();
        var address =
            Regex.Match(await responseMessage.Content.ReadAsStringAsync(), "\"content\":\"(.+?)\"");

        Globals.IpAddress = IPAddress.Parse(address.Groups[1].ToString());
    }
}