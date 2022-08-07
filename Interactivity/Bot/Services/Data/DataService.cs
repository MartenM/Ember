using Discord;
using Interactivity.Data;
using Microsoft.EntityFrameworkCore;

namespace Interactivity.Bot.Services;

public class DataService
{

    public async Task<EmberGuild?> GetGuild(ulong id)
    {
        await using var db = new DiscordContext();

        var guild = await db.Guilds.FirstOrDefaultAsync(guild => guild.GuildId == id);
        return guild;
    }

    public async Task<EmberGuild> CreateGuild(ulong guildId)
    {
        var defaultGuild = new EmberGuild() { GuildId = guildId, Name = "" };
        
        await using var db = new DiscordContext();
        await db.AddAsync(defaultGuild);
        await db.SaveChangesAsync();
        
        return defaultGuild;
    }

    public async Task UpdateGuild(EmberGuild guild)
    {
        await using var db = new DiscordContext();
        db.Update(guild);
        await db.SaveChangesAsync();
    }

    public async Task<EmberMember> GetMember(ulong guildId, ulong memberId)
    {
        await using var db = new DiscordContext();
        var member = await db.Members
            .Include(member => member.IssuedSanctions)
            .ThenInclude(sanction => sanction.Receiver)
            .Include(member => member.ReceivedSanctions)
            .ThenInclude(sanction => sanction.By)
            .FirstOrDefaultAsync(member => member.GuildId == guildId && member.DiscordId == memberId);

        if (member == null)
        {
            var result = await db.Members.AddAsync(new EmberMember()
            {
                DiscordId = memberId,
                GuildId = guildId,
                IssuedSanctions = new List<EmberSanction>(),
                ReceivedSanctions = new List<EmberSanction>(),
            });
            member = result.Entity;
            await db.SaveChangesAsync();
        }

        return member;
    }

    public async Task UpdateMember(EmberMember member)
    {
        await using var db = new DiscordContext();
        db.Update(member);
        await db.SaveChangesAsync();
    }

    public void AddSanction(EmberMember to, EmberMember by, string reason)
    {
        using var db = new DiscordContext();
        db.Sanctions.Add(new EmberSanction()
        {
            ById = by.Id,
            ReceiverId = to.Id,
            Reason = reason
        });
        db.SaveChanges();
    }
}