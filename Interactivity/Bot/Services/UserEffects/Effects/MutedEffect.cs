using Discord;

namespace Interactivity.Bot.Services.Effects;

public class MutedEffect : IUserEffect
{
    public MutedEffect() : base("MUTED")
    {
        
    }

    public override Task Apply(IServiceProvider provider, EffectContext context)
    {
        return Task.CompletedTask;
    }

    public override Task Remove(IServiceProvider provider, EffectContext context)
    {
        throw new NotImplementedException();
    }
}