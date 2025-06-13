using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Help;
using Spectre.Console.Rendering;

internal class CustomHelpProvider : HelpProvider
{
    private readonly HelpProviderStyle? helperStyles;

    public CustomHelpProvider(ICommandAppSettings settings)
        : base(settings)
    {
        helperStyles = settings.HelpProviderStyles;
    }

    public override IEnumerable<IRenderable> GetHeader(ICommandModel model, ICommandInfo? command)
    {
        return new[]
        {
            new Text("--------------------------------------"), Text.NewLine,
            new Text("---        Swedish Test ssn        ---"), Text.NewLine,
            new Text("--------------------------------------"), Text.NewLine,
            Text.NewLine,
        };
    }

    public override IEnumerable<IRenderable> GetDescription(ICommandModel model, ICommandInfo? command)
    {
        return [
          new Text("Get test social security numbers from Skatteverket. "),
          new Text("You can use these as approved test data"),
          Text.NewLine,
          Text.NewLine,
          Text.NewLine
        ];
    }

    public override IEnumerable<IRenderable> GetUsage(ICommandModel model, ICommandInfo? command)
    {
        return [
          new Text("Usage", helperStyles?.Usage?.Header ),
          Text.NewLine,
          new Text("    "),
          new Text("swetestssn"),
          new Text(" "),
          new Text("[Pattern]", helperStyles?.Usage?.RequiredArgument),
          new Text(" "),
          new Text("[options]", helperStyles?.Usage?.Options),
          Text.NewLine
        ];
    }
}
