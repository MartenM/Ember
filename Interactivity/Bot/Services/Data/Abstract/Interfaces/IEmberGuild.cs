using Discord;
using Interactivity.Bot.Abstract.Interfaces;

namespace Interactivity.Bot.Services.Data.Abstract.Interfaces;

public interface IEmberGuild
{
    public ulong GuildId { get; }
    
    // Meta data, not always up-to-date
    public string Name { get; set; }
    
    // Functional data
    public ulong? AuditChannel { get; set; }
    public ulong? LogChannel { get; set; }

    public ulong? ModRole { get; set; }
    public ulong? MutedRole { get; set; }

    public List<IApplicationGuildMember> Members { get; set; }

    public Task Save();

    public Task LogAudit(EmbedBuilder builder);
}