using Discord;
using Discord.Interactions;
using Interactivity.Bot.Interactions.context;
using Interactivity.Bot.Interactions.Precondition;

namespace Interactivity.Bot.Interactions.Commands.Toolbox;

[IsServerModerator]
public class MessageModule : InteractionModuleBase<EmberInteractionContext>
{
    public const string BTN_SEND_MESSAGE = "btn_send_message";
    private const string MODAL_MESSAGE = "modal_send_message";
    
    public class MessageModal : IModal
    {
        public string Title => "Send message";
        
        [InputLabel("Message")]
        [ModalTextInput("message", TextInputStyle.Paragraph, placeholder: "Enter your note here", maxLength: 1000, minLength: 0)]
        public string Message { get; set; }
    }

    [ComponentInteraction($"{BTN_SEND_MESSAGE}:*")]
    public async Task OpenMessageDialog(string discordId)
    {
        await RespondWithModalAsync<MessageModal>($"{MODAL_MESSAGE}:{discordId}");
    }

    [SlashCommand("message", "Used to send a moderator message to a user")]
    public async Task ModMessageCommandHandle(IUser user, string message)
    {
        await SendModMessage(user, message);
    }

    [ModalInteraction($"{MODAL_MESSAGE}:*")]
    public async Task ModalResponse(string discordId, MessageModal modal)
    {
        await SendModMessage(Convert.ToUInt64(discordId), modal.Message);
    }
    
    private async Task SendModMessage(ulong discordId, string message)
    {
        var user = Context.Guild.Users.FirstOrDefault(u => u.Id == Convert.ToUInt64(discordId));
        if (user == null) return;
        await SendModMessage(user, message);
    }

    private async Task SendModMessage(IUser user, string message)
    {
        var messageString = $"**You received a message from one of our moderators:**";
        var builder = new EmbedBuilder()
            .WithAuthor(Context.Guild.Name, Context.Guild.IconUrl)
            .WithColor(Color.LighterGrey)
            .WithFooter("You cannot reply to this message.");

        builder.Description += messageString + "\n";
        builder.Description += message;

        await user.SendMessageAsync(embed: builder.Build());
        await RespondAsync($"The following message has been send to {user.Mention}:\n",
            embed: builder.Build(), ephemeral: true);
    }
}