using Discord;
using Discord.WebSocket;
using Interactivity.Bot.Utilities;
using Interactivity.Data;

namespace Interactivity.Bot.Services.UserServices;

public class MutedUserService
{
    public enum MuteError
    {
        RoleNotSetup,
        RoleCreationFailed,
        NotMuted,
    }

    private DataService _dataService;

    public MutedUserService(DataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<ServiceResult<MuteError>> MuteUser(EmberGuild emberGuild, SocketGuild guild, IGuildUser user, string reason, TimeSpan? duration)
    {
        // Find the muted role if it exists.
        IRole? mutedRole = emberGuild.MutedRole.HasValue ? guild.Roles.FirstOrDefault(role => role.Id == emberGuild.MutedRole) : null;
        
        // First Attempt, role could not be found. Create it instead.
        if (mutedRole == null)
        {
            mutedRole = await SetupMutedRole(emberGuild, guild);

            if (mutedRole == null)
            {
                return ServiceResult<MuteError>.BotError(MuteError.RoleCreationFailed);
            }
        }

        // Add role to user.
        await user.AddRoleAsync(mutedRole);
        
        // Create the message
        var messageString = $"**You have been muted.\nReason: **";
        messageString += reason;
        if (duration.HasValue)
        {
            var readable = String.Format("{0} days, {1} hours, {2} minutes", 
                duration.Value.Days, duration.Value.Hours, duration.Value.Minutes);
            messageString += $"\n**Duration:** {readable}";
        }
        
        // Create the message embed
        var builder = new EmbedBuilder()
            .WithAuthor(guild.Name, guild.IconUrl)
            .WithColor(Color.Red)
            .WithDescription(messageString)
            .WithFooter("You cannot reply to this message.");

        await user.SendMessageAsync(embed: builder.Build());
        
        //TODO: Schedule the mute to be removed.
        return ServiceResult<MuteError>.Success();
    }

    public async Task<ServiceResult<MuteError>> UnmuteUser(EmberGuild emberGuild, IGuild guild, IGuildUser user)
    {
        if (!emberGuild.MutedRole.HasValue)
        {
            return ServiceResult<MuteError>.UserError(MuteError.RoleNotSetup);
        }

        if (!user.RoleIds.Contains(emberGuild.MutedRole.Value))
        {
            return ServiceResult<MuteError>.UserError(MuteError.NotMuted);
        }
        
        await user.RemoveRoleAsync(emberGuild.MutedRole.Value);
        var userMessage = new EmbedBuilder()
            .WithAuthor(guild.Name, guild.IconUrl)
            .WithColor(Color.LighterGrey)
            .WithDescription("**You have been unmuted.**")
            .WithFooter("You cannot reply to this message.");

        await user.SendMessageAsync(embed: userMessage.Build());
        return ServiceResult<MuteError>.Success();
    }

    /// <summary>
    /// Create the muted role and returns it's ID.
    /// This is saved to the database when created.
    ///
    /// TODO: When the role is already present on the server, it's applied to all channels.
    /// </summary>
    /// <returns></returns>
    private async Task<IRole?> SetupMutedRole(EmberGuild emberGuild, SocketGuild guild)
    {
        // Create a role with the same permissions as the everyone role.
        var role = await guild.CreateRoleAsync("Muted", guild.EveryoneRole.Permissions, Color.DarkerGrey, isMentionable: false,
            isHoisted: false);
        
        // Get all channels and apply the role.
        var permissions = MutedPermissions();
        
        foreach (var socketGuildChannel in guild.Channels)
        {
            // Don't await this. This is going to take some time.
            socketGuildChannel.AddPermissionOverwriteAsync(role, permissions);
        }

        emberGuild.MutedRole = role.Id;
        await _dataService.UpdateGuild(emberGuild);
        return role;
    }

    private static OverwritePermissions MutedPermissions()
    {
        return new OverwritePermissions(
            sendMessages: PermValue.Deny,
            sendMessagesInThreads: PermValue.Deny,
            createPublicThreads: PermValue.Deny,
            createPrivateThreads: PermValue.Deny,
            addReactions: PermValue.Deny
        );
    }
}