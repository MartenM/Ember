using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Interactivity.Bot.Interactions.context;

namespace Interactivity.Bot.Interactions.Precondition;

public class IsServerModerator : PreconditionAttribute
{
    public override string ErrorMessage => "Not enough permissions";

    public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        if (context is not EmberInteractionContext eContext)
        {
            return Task.FromResult(
                PreconditionResult.FromError("This interaction can only be executed in EmberGuilds."));
        }

        var guildUser = context.User as SocketGuildUser;
        if (guildUser == null || eContext.EmberGuild == null) return Task.FromResult(
            PreconditionResult.FromError("This interaction can only be executed in EmberGuilds."));

        if (guildUser.GuildPermissions.Administrator) {
            // Admins are always allowed to execute bot commands.
            return Task.FromResult(PreconditionResult.FromSuccess());
        }

        if (!eContext.EmberGuild.ModRole.HasValue)
        {
            return Task.FromResult(
                PreconditionResult.FromError("No moderation role has been configured."));
        }

        if (guildUser.Roles.Any(role => role.Id == eContext.EmberGuild.ModRole.Value))
        {
            return Task.FromResult(PreconditionResult.FromSuccess());
        }
        
        return Task.FromResult(
            PreconditionResult.FromError("Not enough permissions."));
    }
}