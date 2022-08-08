using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Interactivity.Bot.Interactions.context;
using Interactivity.Bot.Interactions.Precondition;
using Interactivity.Bot.Services;
using Interactivity.Data;

namespace Interactivity.Bot.Interactions.Commands;

[IsServerModerator]
public class WarningModule : InteractionModuleBase<EmberInteractionContext>
{
    public const string BTN_SEND_WARNING = "btn_warn";
    private const string MODAL_WARN = "modal_warn";

    private readonly DataService _dataService;

    public WarningModule(DataService dataService)
    {
        _dataService = dataService;
    }
    
    public class WarnModal : IModal
    {
        public string Title => "Send warning to user";

        [InputLabel("Warning message")]
        [ModalTextInput("message", TextInputStyle.Paragraph, placeholder: "Warning reason", maxLength: 200, minLength: 0)]
        public string Reason { get; set; }
    }

    [ComponentInteraction($"{BTN_SEND_WARNING}:*")]
    public async Task WarningButtonResponse(ulong discordId)
    {
        await RespondWithModalAsync<WarnModal>($"{MODAL_WARN}:{discordId}");
    }

    [SlashCommand("warn", "Send a warning to the user.")]
    public async Task WarningCommandHandle(IUser user, string reason)
    {
        await WarnUser(user, reason);
    }

    [ModalInteraction($"{MODAL_WARN}:*")]
    public async Task WarningModalResponse(string discordId, WarnModal modal)
    {
        await WarnUser(Convert.ToUInt64(discordId), modal.Reason);
    }

    private async Task WarnUser(IUser user, string reason)
    {
        var emberMember = await _dataService.GetMember(Context.Guild.Id, user.Id);
        
        // Construct database entry.
        emberMember.ReceivedSanctions.Add(new EmberSanction()
        {
            ById = Context.Executor!.Id,
            type = EmberSanction.Type.Warning,
            Reason = reason,
        });
        await _dataService.UpdateMember(emberMember);
        
        // Send message modal.
        var messageString = $"**You received a warning.\nReason: **";
        messageString += reason;
        
        var builder = new EmbedBuilder()
            .WithCurrentTimestamp()
            .WithAuthor(Context.Guild.Name, Context.Guild.IconUrl)
            .WithColor(Color.Orange)
            .WithDescription(messageString)
            .WithFooter("You cannot reply to this message.");

        await user.SendMessageAsync(embed: builder.Build());
        await RespondAsync($"User has been warned.\nThey received the following message:", embed: builder.Build(), ephemeral: true);
    }
    
    private async Task WarnUser(ulong discordId, string reason)
    {
        var user = Context.Guild.Users.FirstOrDefault(u => u.Id == discordId);
        if (user == null) return;

        await WarnUser(user, reason);
    }
}