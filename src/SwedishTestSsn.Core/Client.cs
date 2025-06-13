using System.Collections.Immutable;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SwedishTestSsn.Core;

public interface IClient
{
    Task<ImmutableArray<string>> GetTestSsns(string pattern, int limit, int offset);
}

public class Client : IClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonDeserializeSettings = new()
    {
        PropertyNameCaseInsensitive = true
    };
    public Client(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("SwedishTestSsnClient");
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public Client(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    /// <summary>
    /// Retrieves data from the Skatteverket API based on the specified pattern, limit, and offset.
    /// </summary>
    /// <remarks>This method constructs a query URL using the provided parameters and sends an HTTP GET
    /// request to the Skatteverket API. Ensure that the <paramref name="pattern"/> parameter is valid and matches the
    /// expected format for the API. The method uses asynchronous operations and should be awaited to avoid blocking the
    /// calling thread.</remarks>
    /// <param name="pattern">A string pattern used to filter the data. Typically represents a test person number.</param>
    /// <param name="limit">The maximum number of records to retrieve. Must be a positive integer.</param>
    /// <param name="offset">The number of records to skip before starting to retrieve results. Must be a non-negative integer.</param>
    /// <returns>A <see cref="Result"/> object containing the deserialized data retrieved from the API.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the response from the Skatteverket API cannot be deserialized into a <see cref="Result"/> object.</exception>
    /// <exception cref="System.Net.Http.HttpRequestException">Thrown when the HTTP request fails.</exception>
    /// <exception cref="System.Threading.Tasks.TaskCanceledException">Thrown when the HTTP request is canceled (e.g., due to a timeout).</exception>
    /// <exception cref="System.ArgumentNullException">Thrown if the request URL is null.</exception>
    private async Task<Result> GetData(string pattern, int limit, int offset)
    {
        var url =
                $"https://skatteverket.entryscape.net/rowstore/dataset/b4de7df7-63c0-4e7e-bb59-1f156a591763" +
                $"?testpersonnummer={pattern}" +
                $"&_limit={limit}" +
                $"&_offset={offset}";
        var response = await _httpClient.GetStringAsync(url).ConfigureAwait(false);

        Result? data = JsonSerializer.Deserialize<Result>(response, _jsonDeserializeSettings);

        return data ?? throw new InvalidOperationException("Failed to deserialize response from Skatteverket.");
    }
    /// <summary>
    /// Retrieves a collection of test Social Security Numbers (SSNs) based on the specified pattern, limit, and offset.
    /// </summary>
    /// <remarks>
    /// This method performs an asynchronous operation to fetch test SSNs. The results are filtered
    /// based on the provided pattern, and pagination is applied using the limit and offset parameters.
    /// Ensure that the input parameters are valid to avoid unexpected behavior.
    /// </remarks>
    /// <param name="pattern">The search pattern used to filter test SSNs. This may include wildcards or specific criteria.</param>
    /// <param name="limit">The maximum number of test SSNs to retrieve. Must be a positive integer.</param>
    /// <param name="offset">The number of records to skip before starting the retrieval. Must be a non-negative integer.</param>
    /// <returns>
    /// An immutable array of strings containing the test SSNs that match the specified criteria.
    /// Returns an empty array if no matching SSNs are found.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown if the response from the Skatteverket API cannot be deserialized into a <see cref="Result"/> object.</exception>
    /// <exception cref="System.Net.Http.HttpRequestException">Thrown when the HTTP request fails.</exception>
    /// <exception cref="System.Threading.Tasks.TaskCanceledException">Thrown when the HTTP request is canceled (e.g., due to a timeout).</exception>
    /// <exception cref="System.ArgumentNullException">Thrown if the request URL is null.</exception>
    public async Task<ImmutableArray<string>> GetTestSsns(string pattern, int limit, int offset)
    {
        Result result = await GetData(pattern, limit, offset).ConfigureAwait(false);
        if (result.Results.Length == 0)
        {
            return [];
        }
        var testSsn = result.Results.Select(ssn => ssn.Testpersonnummer).ToImmutableArray();
        return testSsn;
    }
}
