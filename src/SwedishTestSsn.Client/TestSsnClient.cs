using System.Collections.Immutable;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SwedishTestSsn.Client;

public interface ITestSsnClient
{
    Task<Result<ImmutableArray<string>>> GetTestSsns(string pattern, int limit, int offset, CancellationToken cancellationToken = default);
}

public class TestSsnClient : ITestSsnClient
{
    private const string BaseAddress = "https://skatteverket.entryscape.net/rowstore/dataset/b4de7df7-63c0-4e7e-bb59-1f156a591763";

    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonDeserializeSettings = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public TestSsnClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Configures an <see cref="HttpClient"/> with the JSON accept header expected by Skatteverket.
    /// </summary>
    public static void Configure(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// Retrieves a collection of test Social Security Numbers (SSNs) based on the specified pattern, limit, and offset.
    /// </summary>
    /// <param name="pattern">The search pattern used to filter test SSNs. Regular expressions may be used.</param>
    /// <param name="limit">The maximum number of test SSNs to retrieve. Must be a positive integer.</param>
    /// <param name="offset">The number of records to skip before starting the retrieval. Must be a non-negative integer.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>
    /// A successful result with the matching test SSNs (empty if none match),
    /// or a failed result if the request fails, times out or the response cannot be deserialized.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public async Task<Result<ImmutableArray<string>>> GetTestSsns(string pattern, int limit, int offset, CancellationToken cancellationToken = default)
    {
        var url =
                $"{BaseAddress}" +
                $"?testpersonnummer={Uri.EscapeDataString(pattern)}" +
                $"&_limit={limit}" +
                $"&_offset={offset}";
        try
        {
            var response = await _httpClient.GetStringAsync(url, cancellationToken).ConfigureAwait(false);
            SearchResponse? data = JsonSerializer.Deserialize<SearchResponse>(response, _jsonDeserializeSettings);

            if (data is null)
            {
                return Result<ImmutableArray<string>>.Failure(new Error("Failed to deserialize response from Skatteverket."));
            }

            return Result<ImmutableArray<string>>.Success(
                data.Value.Results.Select(ssn => ssn.Testpersonnummer).ToImmutableArray());
        }
        catch (HttpRequestException ex)
        {
            return Result<ImmutableArray<string>>.Failure(new Error($"Request to Skatteverket failed: {ex.Message}"));
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return Result<ImmutableArray<string>>.Failure(new Error("Request to Skatteverket timed out."));
        }
        catch (JsonException ex)
        {
            return Result<ImmutableArray<string>>.Failure(new Error($"Invalid response from Skatteverket: {ex.Message}"));
        }
    }
}
