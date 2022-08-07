using System.Drawing;
using System.Globalization;
using System.Text;
using Discord;
using Discord.Interactions;
using Interactivity.Bot.Interactions.context;
using Interactivity.Bot.Services;
using Interactivity.Data;
using Color = Discord.Color;

namespace Interactivity.Bot.Interactions.Commands;

public class SanctionModule : InteractionModuleBase<EmberInteractionContext>
{
    private DataService _dataService;

    private const string BTN_SANCTION = "btn_sanction";
    private const string BTN_SET_NOTE = "btn_set_note";
    private const string BTN_TOOLBOX = "btn_user_toolbox";

    public SanctionModule(DataService dataService)
    {
        _dataService = dataService;
    }

    [SlashCommand("sanction", "Base command for sanctions")]
    public async Task SanctionCommand(IUser member, bool showInChannel = false)
    {;
        await DeferAsync(ephemeral: !showInChannel);
        var emberUser = await _dataService.GetMember(Context.Guild.Id, member.Id);

        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithCurrentTimestamp();
        embedBuilder.WithAuthor(member);
        embedBuilder.WithColor(new Color(36, 240, 255));
        embedBuilder.WithTitle("User information");

        embedBuilder.Description += GetSanctionLog(emberUser.ReceivedSanctions) + "\n";
        embedBuilder.Description += "**Note:**\n";
        embedBuilder.Description += emberUser.ModeratorsNote ?? "*Nothing yet*";
        embedBuilder.Description += "\n";
        
        embedBuilder.AddField("Total sanctions", emberUser.ReceivedSanctions.Count, true);
        embedBuilder.AddField("Active sanctions", "...", true);
        embedBuilder.AddField("Last sanction", emberUser.ReceivedSanctions.FirstOrDefault()?.Inserted.ToString(CultureInfo.InvariantCulture) ?? "N/a", true);
        embedBuilder.AddField("Account created on", member.CreatedAt, true);

        var componentBuilder = new ComponentBuilder();
        componentBuilder.WithButton("❌", $"close", ButtonStyle.Secondary);
        componentBuilder.WithButton("Sanction user", $"{BTN_SANCTION}:{member.Id}", ButtonStyle.Danger);
        componentBuilder.WithButton("🛠 Toolbox", $"{BTN_TOOLBOX}:{member.Id}");
        componentBuilder.WithButton("🧾 Set note", $"{BTN_SET_NOTE}:{member.Id}");

        await FollowupAsync(embed: embedBuilder.Build(), components: componentBuilder.Build(), ephemeral: !showInChannel);
    }

    public class NoteModal : IModal
    {
        public string Title => "Add note";

        [InputLabel("Note")]
        [ModalTextInput("user_note", TextInputStyle.Paragraph, placeholder: "Enter your note here", maxLength: 100, minLength: 0)]
        public string Note { get; set; }
    }

    [ComponentInteraction($"{BTN_SET_NOTE}:*")]
    public async Task OpenNoteEditor(string discordId)
    {
        var user = Context.Guild.Users.FirstOrDefault(u => u.Id == Convert.ToUInt64(discordId));
        if (user == null) return;


        await RespondWithModalAsync<NoteModal>($"set_note:{user.Id}");
    }
    
    [ComponentInteraction($"{BTN_TOOLBOX}:*")]
    public async Task OpenToolbox(string discordId)
    {
        var user = Context.Guild.Users.FirstOrDefault(u => u.Id == Convert.ToUInt64(discordId));
        if (user == null) return;

        ComponentBuilder componentBuilder = new ComponentBuilder()
            .WithButton("💬 Send message", "1");

        await RespondAsync($"Please select an action for {user.Mention}", components: componentBuilder.Build(), ephemeral: true);
    }
    
    [ComponentInteraction($"{BTN_SANCTION}:*")]
    public async Task ChooseSanction(string discordId)
    {
        var user = Context.Guild.Users.FirstOrDefault(u => u.Id == Convert.ToUInt64(discordId));
        if (user == null) return;

        ComponentBuilder componentBuilder = new ComponentBuilder()
            .WithButton("⚠ Warn user", "1")
            .WithButton("🔇 Mute user", "2")
            .WithButton("💥 Ban user", "3");

        await RespondAsync($"Please select an action for {user.Mention}", components: componentBuilder.Build(), ephemeral: true);
    }

    [ModalInteraction("set_note:*")]
    public async Task ModalResponse(string discordId, NoteModal modal)
    {
        var user = Context.Guild.Users.FirstOrDefault(u => u.Id == Convert.ToUInt64(discordId));
        if (user == null) return;
        
        var emberMember = await _dataService.GetMember(Context.Guild.Id, user.Id);
        emberMember.ModeratorsNote = modal.Note.Length == 0 ? "" : modal.Note;
        await _dataService.UpdateMember(emberMember);
        await RespondAsync($"📜 Note has been updated to {modal.Note}", ephemeral: true);
    }

    public string GetSanctionLog(List<EmberSanction> sanctions)
    {
        if (sanctions == null) throw new NullReferenceException("Sanctions is NULL");
        
        StringBuilder builder = new StringBuilder();
        builder.AppendLine($"**Last sanctions:**");
        if (sanctions.Count == 0)
        {
            builder.AppendLine("*This user has never received a sanction*");
            return builder.ToString();
        }

        for (int i = 0; i < Math.Min(10, sanctions.Count); i++)
        {
            var sanction = sanctions[i];
            builder.AppendLine($"#{i}: {sanction.Reason}");
            builder.AppendLine($"        Issued by: {MentionUtils.MentionUser(sanction.By.DiscordId)} at {sanction.Inserted}");
        }

        if (sanctions.Count > 10)
        {
            builder.AppendLine("...");
        }

        return builder.ToString();
    }
    
}