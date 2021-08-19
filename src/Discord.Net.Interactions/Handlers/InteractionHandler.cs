using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.WebSocket;

namespace Discord.Net.Interactions.Handlers
{
    /// <summary>
    /// Base class for a command handler
    /// </summary>
    /// <remarks>
    /// Exposes basic command handling helper commands
    /// </remarks>
    public class InteractionHandler<TSlashInfo> : IDisposable
        where TSlashInfo : SlashCommandInfo
    {
        protected readonly ICommandHolder<TSlashInfo> _commandsHolder;
        protected readonly CancellationTokenSource _commandsTokenSource;
        protected readonly DiscordSocketClient _client;
        
        public InteractionHandler(DiscordSocketClient client, ICommandHolder<TSlashInfo> commandsHolder)
        {
            _commandsTokenSource = new CancellationTokenSource();
            _commandsHolder = commandsHolder;
            _client = client;
        }

        /// <summary>
        /// Setup events for handling
        /// </summary>
        protected virtual void SetupEvents()
        {
            _client.InteractionCreated += HandleInteractionCreated;
        }
        
        /// <summary>
        /// Handle InteractionCreated event
        /// </summary>
        /// <remarks>
        /// Calls HandleCommand if the interaction is a SocketSlashCommand and found in commands collection
        /// </remarks>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual Task HandleInteractionCreated(SocketInteraction arg)
        {
            if (arg is SocketSlashCommand command)
            {
                HeldSlashCommand<TSlashInfo>? heldCommand = _commandsHolder.TryGetSlashCommand(command.Data.Name);

                if (heldCommand != null)
                {
                    return heldCommand.Executor.TryExecuteCommand(
                        heldCommand.Info,
                        command,
                        _commandsTokenSource.Token);
                }

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Registers commands and events
        /// </summary>
        /// <param name="token"></param>
        public Task StartAsync(CancellationToken token = new CancellationToken())
        {
            SetupEvents();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops all current running handlers
        /// and unregister commands
        /// </summary>
        /// <param name="token"></param>
        public Task StopAsync(CancellationToken token = new CancellationToken())
        {
            _client.InteractionCreated -= HandleInteractionCreated;
            
            _commandsTokenSource.Cancel();
            token.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _commandsTokenSource.Dispose();
        }
    }
}