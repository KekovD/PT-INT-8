using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Initiator.Services.Interfaces;
using Newtonsoft.Json;

namespace Initiator.Services;

public class HttpClientOperations : IHttpOperations
{
    private readonly HttpClient _httpClient;

    public HttpClientOperations(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task PostAsJsonAsync<T>(string uri, T value, CancellationToken cancellationToken)
    {
        if (uri == null)
            throw new ArgumentNullException(nameof(uri));

        var content = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
    }
}