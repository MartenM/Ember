using ApplicationWrap;
using ApplicationWrap.Commands;
using Interactivity.Bot;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Starting...");

var config = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("config.json").Build();

var token = config.GetValue<string>("discord_token");
if (token == null)
{
    Console.WriteLine("[ERROR] A valid token must be provided in the DISCORD_TOKEN environment variable.");
    return;
}

var bot = new DiscordBot(token);
await bot.Start();

var clManager = new ClManager(bot);
clManager.RegisterCommand(new StopCommand(bot));
clManager.RegisterCommand(new SetupCommand(bot));
clManager.RegisterCommand(new MakeSayCommand(bot));
clManager.RegisterCommand(new DebugCommand(bot));
clManager.Run();
