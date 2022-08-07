using Discord;
using Discord.WebSocket;
using Interactivity.Bot.Abstract.Interfaces;
using Interactivity.Bot.Services.Data.Abstract.Interfaces;
using Interactivity.Data;

namespace Interactivity.Bot.Services.Implementation;

public class CraftGuild : IEmberGuild
{
    private IDiscordClient _client;
    private Guild Handle { get; }

    public CraftGuild(IDiscordClient client, Guild handle)
    {
        _client = client;
        Handle = handle;
    }


    public ulong GuildId => Handle.GuildId;

    public string Name
    {
        get => Handle.Name;
        set => Handle.Name = value;
    }

    public ulong? AuditChannel
    {
        get => Handle.AuditChannel;
        set => Handle.AuditChannel = value;
    }

    public ulong? LogChannel
    {
        get => Handle.LogChannel;
        set => Handle.LogChannel = value;
    }

    public ulong? ModRole
    {
        get => Handle.ModRole;
        set => Handle.ModRole = value;
    }

    public ulong? MutedRole
    {
        get => Handle.MutedRole;
        set => Handle.MutedRole = value;
    }

    public List<IEmberGuildMember> Members
    {
        get
        {
            return new List<IEmberGuildMember>();
        }
        set
        {
            
        }
    }
    
    public async Task Save()
    {
        await using var db = new DiscordContext();
        db.Update(Handle);
        await db.SaveChangesAsync();
    }

    public async Task LogAudit(EmbedBuilder builder)
    {
        if (!Handle.AuditChannel.HasValue) return;
        var channel = await _client.GetChannelAsync(Handle.AuditChannel.Value) as ISocketMessageChannel;

        await channel.SendMessageAsync(embed: builder.Build());
    }
}