using System.Reflection;
using ApplicationWrap;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Interactivity.Bot.Interactions;
using Interactivity.Bot.Services;
using Interactivity.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Interactivity.Bot
{
    public class DiscordBot : ICommandLineEntity
    {
        public readonly DiscordSocketClient Client;
        private string _token;

        public static bool IsDebug = false;
        private bool _ready;
        private bool _alive;

        private CommandHandler _commandHandler;
        
        // Services
        private LoggingService _loggingService;
        private InteractionService _interactionService;
        private DataService _dataService;

        public DiscordBot(string token)
        {
            #if DEBUG
            IsDebug = true;
            #endif
            
            _token = token;
            Client = new DiscordSocketClient();
            Client.Ready += OnReady;

            _interactionService = new InteractionService(Client);
            _dataService = new DataService();

            _loggingService = new LoggingService(Client, _interactionService);
        }

        public async Task Start()
        {
            _alive = true;
            
            // Build service provider.
            var serviceProvider = new ServiceCollection()
                .AddSingleton(_loggingService)
                .AddSingleton(_dataService)
                .AddDbContext<DiscordContext>()
                .BuildServiceProvider();

            // Discover interaction modules
            await _interactionService.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: serviceProvider);

            // Create the command handler.
            _commandHandler = new CommandHandler(this, serviceProvider, _interactionService, _loggingService);

            // Start the client
            await Client.LoginAsync(TokenType.Bot, _token);
            await Client.StartAsync();
        }

        public async Task Stop()
        {
            _alive = false;
            _ready = false;

            await Client.StopAsync();
        }

        private Task OnReady()
        {
            _ready = true;
            return Task.CompletedTask;
        }

        public bool isAlive()
        {
            return _alive;
        }

        public async Task<bool> InstallBot()
        {
            if (!_ready) return false;
            await _interactionService.RegisterCommandsGloballyAsync();
            return true;
        }

        /// <summary>
        /// Used to execute debugging code. Should only be used for debugging code and should remain
        /// empty on PR request.
        /// </summary>
        public async Task ExecuteDebugCode()
        {
            var appliedBy = await _dataService.GetMember(326718078438080532, 221359027802603520);
            var member = await _dataService.GetMember(326718078438080532, 992532939554426990);

            _dataService.AddSanction(member, appliedBy, "You don't even know...");
            Console.WriteLine("Applying sanction Done!");
        }
    }
}
