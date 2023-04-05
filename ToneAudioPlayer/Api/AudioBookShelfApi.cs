using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using OperationResult;
using ToneAudioPlayer.Api;
using static OperationResult.Helpers;

/*
 * what would I change:
 * Ok() => SuccessResult()
 * Error() => ErrorResult()
 */


namespace ToneAudioPlayer.Services;

public class AudioBookShelfApi
{
    private readonly HttpClient _httpClient;
    public bool HasBaseAddress => !string.IsNullOrEmpty(_httpClient.BaseAddress?.ToString());

    public Uri? BaseAddress
    {
        get => _httpClient.BaseAddress;
        set => _httpClient.BaseAddress = value;
    }

    public AudioBookShelfApi(HttpClient httpClient)
    {
        _httpClient = httpClient;
        //         httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));  
        // client.DefaultRequestHeaders.ConnectionClose = true;
        // request.Headers.Authorization = new BasicAuthenticationHeaderValue("username", "password");
        // httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {Base64Encode($"{Username}:{Password}")}");
    }

    public async Task<Status<HttpResponseMessage>> AuthenticateAsync(IApiCredentials credentials,
        CancellationToken? cancellationToken = null)
    {
        credentials.ModifyHeaders(_httpClient);
        var response = await _httpClient.GetAsync(credentials.UrlString, cancellationToken ?? CancellationToken.None);
        if (response.IsSuccessStatusCode)
        {
            return Ok();
        }

        return Error(response);
    }
}