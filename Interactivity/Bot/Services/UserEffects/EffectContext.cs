using Discord;
using Interactivity.Data;

namespace Interactivity.Bot.Services;

public class EffectContext
{
    public EffectContext(IGuild guild, EmberGuild emberGuild, IGuildUser user, EmberMember emberMember)
    {
        Guild = guild;
        EmberGuild = emberGuild;
        User = user;
        EmberMember = emberMember;
    }

    public IGuild Guild { get; }
    public EmberGuild EmberGuild { get; }
    
    public IGuildUser User { get; }
    public EmberMember EmberMember { get; }
}