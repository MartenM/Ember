using Discord;

namespace Interactivity.Bot.Services;

public abstract class IUserEffect
{
    protected IUserEffect(string type)
    {
        Type = type;
    }

    public string Type { get; }

    public abstract Task Apply(IServiceProvider provider, EffectContext context);

    public abstract Task Remove(IServiceProvider provider, EffectContext context);
}