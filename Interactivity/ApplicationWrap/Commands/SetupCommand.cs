using Interactivity.Bot;

namespace ApplicationWrap.Commands;

public class SetupCommand : CLCommand
{
    private DiscordBot _bot;
    
    public SetupCommand(DiscordBot bot) : base("setup", "Setup the bot. Registers databases and commands.")
    {
        _bot = bot;
    }

    public override async Task Execute(string[] args)
    {
        if (await _bot.InstallBot())
        {
            Console.WriteLine("Bot has been setup successfully.");
        }
        else
        {
            Console.WriteLine("Failed to setup the bot. Please try again after the bot has been logged in correctly.");
        }
    }
}