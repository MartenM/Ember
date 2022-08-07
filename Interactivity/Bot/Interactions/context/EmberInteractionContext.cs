using Discord.Interactions;
using Discord.WebSocket;
using Interactivity.Data;

namespace Interactivity.Bot.Interactions.context;

public class EmberInteractionContext : SocketInteractionContext
{
    public EmberGuild? EmberGuild { get; }
    public EmberMember? Executor { get; }
    
    public EmberInteractionContext(DiscordSocketClient client, SocketInteraction interaction, EmberGuild? emberGuild, EmberMember? executor) : base(client, interaction)
    {
        EmberGuild = emberGuild;
        Executor = executor;
    }
}