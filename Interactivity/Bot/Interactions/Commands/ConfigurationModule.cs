using Discord;
using Discord.Interactions;
using Interactivity.Bot.Interactions.context;
using Interactivity.Bot.Services;

namespace Interactivity.Bot.Interactions.Commands;

[RequireUserPermission(GuildPermission.Administrator)]
[Group("config", "Configure the Ember bot")]
public class ConfigurationModule : InteractionModuleBase<EmberInteractionContext>
{
    private readonly DataService _dataService;

    public ConfigurationModule(DataService dataService)
    {
        _dataService = dataService;
    }

    [SlashCommand("set-role", "Set the role used for moderation.")]
    public async Task SetModerationRole(IRole? role = null)
    {
        var guild = Context.EmberGuild!;
        guild.ModRole = role?.Id ?? null;
        await _dataService.UpdateGuild(guild);
        await RespondAsync($"Moderation role has been set to {role?.Mention ?? "`None`"}", ephemeral: true);
    }
    
    [SlashCommand("audit-channel", "Sets the channel for audit messages. Leave empty to reset.")]
    public async Task SetAuditChannel(ITextChannel? channel = null)
    {
        var guild = Context.EmberGuild!;
        guild.AuditChannel = channel?.Id ?? null;
        await _dataService.UpdateGuild(guild);
        await RespondAsync($"Audit channel has been set to: {channel?.Mention ?? "`None`"}", ephemeral: true);
    }
    
    [SlashCommand("log-channel", "Sets the channel for audit messages. Leave empty to reset.")]
    public async Task SetLogChannel(ITextChannel? channel = null)
    {
        var guild = Context.EmberGuild!;
        guild.LogChannel = channel?.Id ?? null;
        await _dataService.UpdateGuild(guild);
        await RespondAsync($"Logging channel has been set to: {channel?.Mention ?? "`None`"}", ephemeral: true);
    }
}