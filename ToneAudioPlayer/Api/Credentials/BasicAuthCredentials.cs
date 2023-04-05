using System;
using System.Net.Http;
using System.Text;
using ToneAudioPlayer.Services;

namespace ToneAudioPlayer.Api.Credentials;

public class BasicAuthCredentials: IApiCredentials
{
    public string UrlString { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";

    public void ModifyHeaders(HttpClient client)
    {
        client.DefaultRequestHeaders.Add("Authorization", $"Basic {Base64Encode($"{Username}:{Password}")}");
    }

    private static string Base64Encode(string s)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
    }
}