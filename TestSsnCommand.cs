using Spectre.Console.Cli;
using Spectre.Console;
using Spectre.Console.Json;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SwedishTestSsn;

internal sealed class TestSsnCommand : AsyncCommand<TestSsnCommand.Settings>
{
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

    [Description("testar beskrivning")]
    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        var result = await GetData(settings);

        if (result is null)
            return 1;

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

    private static void PrintText(Result result)
    {
        foreach (var item in result.Results)
        {
            Console.WriteLine(item.Testpersonnummer);
        }
    }

    private static void PrintJson(Result result)
    {
        var items = result.Results.Select(x => x.Testpersonnummer);
        var json = JsonSerializer.Serialize(items);
        var jsonContent = new JsonText(json);

        AnsiConsole.Write(jsonContent);
    }

    private static async Task<Result?> GetData(Settings settings)
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
          new MediaTypeWithQualityHeaderValue("application/json"));
        var data = await client.GetStringAsync(
            $"https://skatteverket.entryscape.net/rowstore/dataset/b4de7df7-63c0-4e7e-bb59-1f156a591763?testpersonnummer={settings.Pattern}&_limit={settings.Limit}&_offset={settings.Offset}");
        var jsonSettings = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        var result = JsonSerializer.Deserialize<Result>(data, jsonSettings);

        return result;
    }
}
