using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Interactivity.Bot;
using Interactivity.Bot.Interactions.context;
using Interactivity.Bot.Services;
using Interactivity.Data;
using Microsoft.Extensions.DependencyInjection;

public class CommandHandler
{
    private DiscordBot _bot;
    private ServiceProvider _serviceProvider;
    private InteractionService _interactionService;
    private LoggingService _loggingService;
    public CommandHandler(DiscordBot bot, ServiceProvider serviceProvider, InteractionService interactionService, LoggingService loggingService)
    {
        _bot = bot;
        _serviceProvider = serviceProvider;
        _interactionService = interactionService;
        _loggingService = loggingService;

        _bot.Client.InteractionCreated += HandleInteraction;
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        EmberGuild? emberGuild = null;
        EmberMember? emberMember = null;

        var database = _serviceProvider.GetService<DataService>();
        if (database == null)
        {
            await _loggingService.LogAsync("CommandHandler",
                new LogMessage(LogSeverity.Error, "CommandHandler", "Failed to initialize the database."));
            return;
        }

        if (interaction.GuildId.HasValue)
        {
            emberGuild = await database.GetGuild(interaction.GuildId.Value);
            if (emberGuild == null)
            {
                // Empty guild. Create it instead.
                emberGuild = await database.CreateGuild(interaction.GuildId.Value);
            }
            
            // Get the ember guild member.
            emberMember = await database.GetMember(interaction.GuildId.Value, interaction.User.Id);
        }

        var ctx = new EmberInteractionContext(_bot.Client, interaction, emberGuild, emberMember);
        var result = await _interactionService.ExecuteCommandAsync(ctx, services: _serviceProvider);
    }
}