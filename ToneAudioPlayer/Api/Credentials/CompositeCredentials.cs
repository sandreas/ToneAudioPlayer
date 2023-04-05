using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using ToneAudioPlayer.Services;

namespace ToneAudioPlayer.Api.Credentials;

public class CompositeCredentials: IApiCredentials
{
    public string UrlString { get; set; } = "";
    
    private readonly IEnumerable<IApiCredentials> _credentials;

    public CompositeCredentials(IEnumerable<IApiCredentials> credentials)
    {
        _credentials = credentials;
    }

    

    public void ModifyHeaders(HttpClient client)
    {
        foreach (var c in _credentials)
        {
            c.ModifyHeaders(client);
        }
    }
}