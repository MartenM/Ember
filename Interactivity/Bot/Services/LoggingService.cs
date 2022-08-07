using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Interactivity.Bot.Services;

public class LoggingService
{
    public LoggingService(DiscordSocketClient client, InteractionService interactionService)
    {
        client.Log += LogClientAsync;
        interactionService.Log += LogInteractionsAsync;
    }

    public Task LogAsync(string sender, LogMessage message)
    {
        Console.WriteLine($"[{sender} / {message.Severity}] {message}");
        return Task.CompletedTask;
    }

    private Task LogClientAsync(LogMessage message)
    {
        return LogAsync("Client", message);
    }

    private Task LogInteractionsAsync(LogMessage message)
    {
        return LogAsync("Interactions", message);
    }

    private Task LogCommandAsync(LogMessage message)
    {
        return LogAsync("Commands", message);
    }

}