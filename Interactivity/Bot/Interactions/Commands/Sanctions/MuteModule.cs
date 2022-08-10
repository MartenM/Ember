using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Interactivity.Bot.Interactions.context;
using Interactivity.Bot.Interactions.Precondition;
using Interactivity.Bot.Services;
using Interactivity.Bot.Services.UserServices;
using Interactivity.Bot.Utilities;

namespace Interactivity.Bot.Interactions.Commands.Sanctions;

[IsServerModerator]
public class MuteModule : InteractionModuleBase<EmberInteractionContext>
{
    private MutedUserService _mutedUserService;

    public MuteModule(MutedUserService mutedUserService)
    {
        _mutedUserService = mutedUserService;
    }

    [SlashCommand("mute", "Mute an user for a specified amount of time.")]
    public async Task MuteCommandHandler(IGuildUser user, string reason, string duration = null)
    {
        await MuteUser(user, reason, duration);
    }

    [SlashCommand("unmute", "Unmute an user")]
    public async Task UnmuteCommandHandler(IGuildUser user)
    {
        await UnmuteUser(user);
    }

    private async Task MuteUser(IGuildUser user, string reason, string? durationInput = null)
    {
        if (user.IsBot)
        {
            await RespondAsync("Bots cannot be muted.", ephemeral: true);
            return;
        }

        if (Context.EmberGuild == null) return;

        // Parse the duration if any has been given.
        TimeSpan? duration = null;
        if (durationInput != null)
        {
            try
            {
                duration = DurationParser.FromString(durationInput);
            }
            catch (DurationParser.UnkownQuantifierException ex)
            {
                await RespondAsync(
                    $"The quantifier `{ex.Quantifier}` is unknown. Please see this full list of time quantifiers:",
                    embed: DurationParser.QuantifierEmbed().Build());
                return;
            }
        }
        
        // Defer from here.
        await DeferAsync();
        var result = await _mutedUserService.MuteUser(Context.EmberGuild!, Context.Guild, user, reason, duration);

        var embed = new EmbedBuilder();
        if (result.State == ServiceResultState.Success)
        {
            embed.WithColor(Color.Green);
            if (duration.HasValue)
            {
                embed.WithDescription($"{user.Mention} has been muted for {duration.Value.Days}");
            }
            else
            {
                embed.WithDescription($"{user.Mention} has been muted permanently.");
            }

            await FollowupAsync(embed: embed.Build());
        }

        embed.Color = result.State == ServiceResultState.UserError ? Color.Orange : Color.Red;
        switch (result.Error)
        {
            case MutedUserService.MuteError.RoleNotSetup:
                embed.WithDescription("The mute role has not been setup properly.");
                break;
            case MutedUserService.MuteError.RoleCreationFailed:
                embed.WithDescription("Failed to create the muted role. Please try again later.");
                break;
            case MutedUserService.MuteError.NotMuted:
            default:
                throw new ArgumentOutOfRangeException();
        }

        await FollowupAsync(embed: embed.Build());
    }

    private async Task UnmuteUser(IGuildUser user)
    {
        var result = await _mutedUserService.UnmuteUser(Context.EmberGuild!, Context.Guild, user);

        var embed = new EmbedBuilder();
        if (result.State == ServiceResultState.Success)
        {
            embed.WithColor(Color.Green);
            embed.WithDescription($"{user.Mention} has been un-muted.");

            await RespondAsync(embed: embed.Build());
        }
        
        embed.Color = result.State == ServiceResultState.UserError ? Color.Orange : Color.Red;
        switch (result.Error)
        {
            case MutedUserService.MuteError.RoleNotSetup:
            case MutedUserService.MuteError.RoleCreationFailed:
                embed.WithDescription("The mute role has not been setup properly.");
                break;
            case MutedUserService.MuteError.NotMuted:
                embed.WithDescription("That user is currently not muted.");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        await RespondAsync(embed: embed.Build());
    }
}