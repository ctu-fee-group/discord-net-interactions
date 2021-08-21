using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.Net.Interactions.CommandsInfo;
using Discord.Net.Interactions.Executors;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Discord.Net.Interactions.Example.Commands
{
    public class ControlCommandGroup : ICommandGroup
    {
        private readonly CommandsOptions _options;
        private readonly ILogger _logger;
        private readonly StopBot _stopBot;
        
        public ControlCommandGroup(ILogger<ControlCommandGroup> logger, StopBot stopBot, IOptions<CommandsOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _stopBot = stopBot;
        }

        private Task HandleQuit(IDiscordInteraction interaction, CancellationToken cancellationToken)
        {
            _stopBot.Invoke();
            return interaction.FollowupAsync("Goodbye");
        }
        
        public Task SetupCommandsAsync(IInteractionHolder holder, CancellationToken token = new CancellationToken())
        {
            // Create quit command info
            SlashCommandInfo info = new SlashCommandInfoBuilder()
                .WithHandler(HandleQuit)
                .WithBuilder(new SlashCommandBuilder()
                    .WithName("quit")
                    .WithDescription("Exit the bot application"))
                .WithGuild(_options.GuildId)
                .Build();
            
            // Create executor along with Deferring ephemerally and logging in case of an error
            IInteractionExecutor executor = new InteractionExecutorBuilder()
                .WithLogger(_logger)
                .WithDeferMessage()
                .Build();

            holder.AddInteraction(info, executor);
            return Task.CompletedTask;
        }
    }
}