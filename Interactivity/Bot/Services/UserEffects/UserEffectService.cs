using Discord;
using Discord.WebSocket;

namespace Interactivity.Bot.Services;

public class UserEffectService
{
    private DiscordSocketClient _client;
    private IServiceProvider _provider;

    public UserEffectService(IServiceProvider provider)
    {
        
    }


    public async Task ApplyEffect(EffectContext context, IUserEffect effect)
    {
        await effect.Apply(_provider, context);
    }

    public async Task RemoveEffect(EffectContext context, IUserEffect effect)
    {
        await effect.Remove(_provider, context);
    }
    
}