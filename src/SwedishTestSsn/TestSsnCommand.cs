using Spectre.Console.Cli;
using Spectre.Console;
using System.ComponentModel;
using System.Text.Json;
using System.Collections.Immutable;
using SwedishTestSsn.Client;

namespace SwedishTestSsn;

internal sealed class TestSsnCommand : AsyncCommand<TestSsnCommand.Settings>
{
    private readonly JsonSerializerOptions _jsonSerializeSettings;
    private readonly TestSsnClient _ssnClient;

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
        public int Offset { get; init; }

        [Description("output as json")]
        [DefaultValue(false)]
        [CommandOption("--json")]
        public bool Json { get; init; }
    }

    public TestSsnCommand()
    {
        _jsonSerializeSettings = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        HttpClient client = new();
        TestSsnClient.Configure(client);

        _ssnClient = new TestSsnClient(client);
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

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var response = await _ssnClient
            .GetTestSsns(settings.Pattern, settings.Limit, settings.Offset, cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccess)
        {
            AnsiConsole.MarkupLineInterpolated($"[red]Error with skatteverket api: {response.Error.Message}[/]");
            return 1;
        }

        var result = response.Value;

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
}
