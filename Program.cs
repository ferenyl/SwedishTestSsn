using Spectre.Console.Cli;
using Spectre.Console.Cli.Help;

var app = new CommandApp<SwedishTestSsn.TestSsnCommand>();

app.Configure(config =>
{
    config.SetHelpProvider(new CustomHelpProvider(config.Settings));

    config.AddCommand<SwedishTestSsn.TestSsnCommand>("swetestssn")
      .WithDescription("Get swedish test social security numbers from The Swedish Tax Agency (Skatteverket). Using their publi API")
      .WithExample("swetestssn", "^1988.*", "--limit", "10")
      .WithExample("swetestssn", "^198807.*", "--limit", "10", "--json")
      .WithExample("swetestssn", "^1988.*", "--limit", "10", "--offset", "10");

});

return app.Run(args);
