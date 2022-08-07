using Discord.WebSocket;
using Interactivity.Bot;

namespace ApplicationWrap.Commands;

public class MakeSayCommand : CLCommand
{
    private DiscordBot _bot;
    private ulong? _oldChannel = null;
    
    public MakeSayCommand(DiscordBot bot) : base("say", "Say something in a channel.")
    {
        _bot = bot;
    }

    public override async Task Execute(string[] args)
    {
        ulong channelId = 0;
        var newId = ulong.TryParse(args[0], out channelId);

        if (!newId && _oldChannel != null)
        {
            channelId = _oldChannel.Value;
        }

        if (channelId == 0)
        {
            Console.WriteLine("A channel is required. Please us it as first argument.");
            return;
        }

        var channel = _bot.Client.GetChannel(channelId) as ISocketMessageChannel;
        if (channel == null)
        {
            Console.WriteLine($"Channel {channelId} was not found.");
            return;
        }
        
        _oldChannel = channelId;
        await channel.SendMessageAsync(string.Join(" ", args, newId ? 1 : 0, args.Length - (newId ? 1 : 0)));
    }
}