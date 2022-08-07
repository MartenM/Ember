using Interactivity.Bot;

namespace ApplicationWrap.Commands;

public class DebugCommand : CLCommand
{
    private DiscordBot _bot;
    
    public DebugCommand(DiscordBot bot) : base("debug", "Used to execute debugging code.")
    {
        _bot = bot;
    }

    public override async Task Execute(string[] args)
    {
        await _bot.ExecuteDebugCode();
        Console.WriteLine("Debug command has been executed.");
    }
}