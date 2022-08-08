using Discord;
using Discord.Interactions;
using Interactivity.Bot.Interactions.context;
using Interactivity.Bot.Interactions.Precondition;
using Interactivity.Bot.Services;

namespace Interactivity.Bot.Interactions.Commands;

[IsServerModerator]
public class DashboardModule : InteractionModuleBase<EmberInteractionContext>
{
    private const string BUTTON_CONFIGURE_SERVER = "btn_configure";

    private DataService _dataService;

    public DashboardModule(DataService dataService)
    {
        _dataService = dataService;
    }
    
    [SlashCommand("dashboard", "Display the current settings and other useful information.")]
    public async Task DisplayInfo()
    {
        EmbedBuilder builder = new EmbedBuilder();
        builder.WithCurrentTimestamp();
        builder.WithTitle($"Ember guild");
        builder.Description += "**Moderation role:** " + (Context.EmberGuild!.ModRole != null ? MentionUtils.MentionRole(Context.EmberGuild!.ModRole.Value) : "Not set") + "\n";
        builder.Description += "**Audit Channel:** " + (Context.EmberGuild!.AuditChannel != null ? MentionUtils.MentionChannel(Context.EmberGuild!.AuditChannel.Value) : "Not set") + "\n";
        builder.Description += "**Moderation role:** " + (Context.EmberGuild!.LogChannel != null ? MentionUtils.MentionChannel(Context.EmberGuild!.LogChannel.Value) : "Not set") + "\n";

        ComponentBuilder componentBuilder = new ComponentBuilder();
        componentBuilder.WithButton("🛠 Configure server", BUTTON_CONFIGURE_SERVER, ButtonStyle.Secondary);

        await RespondAsync(embed: builder.Build(), components: componentBuilder.Build(), ephemeral: true);
    }

    [ComponentInteraction(BUTTON_CONFIGURE_SERVER)]
    public async Task DisplayConfigurationOptions()
    {
        await RespondAsync(embed: new EmbedBuilder().WithDescription("Not implemented yet :(").Build(), ephemeral: true);
    }

}