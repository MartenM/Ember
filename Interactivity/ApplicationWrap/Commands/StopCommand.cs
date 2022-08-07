using Interactivity.Bot;

namespace ApplicationWrap.Commands;

public class StopCommand : CLCommand
{
    private DiscordBot _bot;
    
    public StopCommand(DiscordBot bot) : base("stop", "Stops the discord bot and shuts down the program.")
    {
        _bot = bot;
    }

    public override async Task Execute(string[] args)
    {
        await _bot.Stop();
    }
}