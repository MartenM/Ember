using Discord;
using Discord.Interactions;
using Interactivity.Bot.Interactions.Commands.Toolbox;
using Interactivity.Bot.Interactions.context;
using Interactivity.Bot.Interactions.Precondition;
using Interactivity.Bot.Services;

namespace Interactivity.Bot.Interactions.Commands;

[IsServerModerator]
public class ToolboxModule : InteractionModuleBase<EmberInteractionContext>
{
    public const string BTN_TOOLBOX = "btn_user_toolbox";

    private DataService _dataService;

    public ToolboxModule(DataService dataService)
    {
        _dataService = dataService;
    }

    [ComponentInteraction($"{BTN_TOOLBOX}:*")]
    public async Task OpenToolbox(string discordId)
    {
        var user = Context.Guild.Users.FirstOrDefault(u => u.Id == Convert.ToUInt64(discordId));
        if (user == null) return;

        var componentBuilder = new ComponentBuilder()
            .WithButton("💬 Send message", $"{MessageModule.BTN_SEND_MESSAGE}:{discordId}");

        await RespondAsync($"Please select an action for {user.Mention}", components: componentBuilder.Build(), ephemeral: true);
    }
}