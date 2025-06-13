using Spectre.Console.Cli;
using Spectre.Console;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Collections.Immutable;
using SwedishTestSsn.Core;

namespace SwedishTestSsn;

internal sealed class TestSsnCommand : AsyncCommand<TestSsnCommand.Settings>
{
    private readonly JsonSerializerOptions _jsonSerializeSettings;
    private readonly HttpClient _client;
    private readonly Client _ssnClient;

    public sealed class Settings : CommandSettings
    {
        [Description("Test social security number. Regular expressions may be used")]
        [CommandArgument(0, "[Pattern]")]
        [DefaultValue(".*")]
        public string Pattern { get; init; } = ".*";

        [CommandOption("-l|--limit")]
        [DefaultValue(100)]
        public int Limit { get; init; } = 100;

        [CommandOption("-o|--offset")]
        [DefaultValue(0)]
        public int Offset { get; init; } = 0;

        [Description("output as json")]
        [DefaultValue(false)]
        [CommandOption("--json")]
        public bool Json { get; init; } = false;
    }

    public TestSsnCommand() : base()
    {
        

        _jsonSerializeSettings = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        _client = new();
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        _ssnClient = new Client(_client);
    }

    public override async Task<int> ExecuteAsync(
        [NotNull] CommandContext context,
        [NotNull] Settings settings)
    {
        var result = await GetData(settings);

        if (result.Length == 0)
        {
            AnsiConsole.MarkupLine($"[red]No result for pattern: {settings.Pattern}[/]");
            return 1;
        }

        if (settings.Json)
        {
            PrintJson(result);
        }
        else
        {
            PrintText(result);
        }

        return 0;
    }

    private static void PrintText(ImmutableArray<string> result)
    {
        foreach (var item in result.AsSpan())
        {
            Console.WriteLine(item);
        }
    }

    private void PrintJson(ImmutableArray<string> result)
    {
        var json = JsonSerializer.Serialize(result, _jsonSerializeSettings);
        Console.WriteLine(json);
    }

    private async Task<ImmutableArray<string>> GetData(Settings settings)
    {
        try
        {
            ImmutableArray<string> data = await _ssnClient
                .GetTestSsns(settings.Pattern, settings.Limit, settings.Offset)
                .ConfigureAwait(false);

            return data;
        }
        catch (Exception)
        {
            AnsiConsole.MarkupLine($"[red]Error with skatteverket api[/]");
            return [];
        }
    }
}
