using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.Net.Interactions.Commands;
using Discord.Net.Interactions.CommandsInfo;
using Discord.Net.Interactions.Components;
using Discord.Net.Interactions.Executors;
using Discord.Rest;
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
        private readonly IComponentHelper _componentHelper;

        public ControlCommandGroup(ILogger<ControlCommandGroup> logger, StopBot stopBot,
            IOptions<CommandsOptions> options, IComponentHelper componentHelper)
        {
            _componentHelper = componentHelper;
            _logger = logger;
            _options = options.Value;
            _stopBot = stopBot;
        }

        private async Task HandleQuit(SocketInteraction interaction, CancellationToken cancellationToken)
        {
            IMessage message = await interaction.FollowupChoiceAsync("Are you sure?", ephemeral: true,
                choices: new[] { ("Yes", "yes"), ("No", "no") });

            _componentHelper.ForMessageChoice(message, interaction.User,
                builder => builder
                    .WithLogger(_logger)
                    .WithThreadPool()
                    .WithDeferMessage(),
                ("yes", HandleConfirmQuit),
                ("no", HandleRejectQuit));
        }

        private Task HandleConfirmQuit(SocketInteraction interaction, CancellationToken cancellationToken)
        {
            _stopBot.Invoke();
            return interaction.FollowupAsync("Quitting!", ephemeral: true);
        }

        private Task HandleRejectQuit(SocketInteraction interaction, CancellationToken cancellationToken)
        {
            return interaction.FollowupAsync("Aborting!", ephemeral: true);
        }

        public Task SetupCommandsAsync(IInteractionHolder holder, CancellationToken token = new CancellationToken())
        {
            // Create quit command info
            SlashCommandInfo info = new SlashCommandInfoBuilder()
                .WithHandler(HandleQuit)
                .WithBuilder(new SlashCommandBuilder()
                    .WithName("quit")
                    .WithDescription("Exit the bot application"))
                .SetGlobal(false)
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