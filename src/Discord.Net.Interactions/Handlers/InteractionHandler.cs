using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.Net.Interactions.InteractionMatchers;
using Discord.WebSocket;

namespace Discord.Net.Interactions.Handlers
{
    /// <summary>
    /// Base class for a command handler
    /// </summary>
    /// <remarks>
    /// Exposes basic command handling helper commands
    /// </remarks>
    public class InteractionHandler : IDisposable
    {
        protected readonly IInteractionHolder _interactionsHolder;
        protected readonly CancellationTokenSource _commandsTokenSource;
        protected readonly DiscordSocketClient _client;

        protected readonly IProvider<IInteractionMatcher> _interactionMatcherProvider;

        public InteractionHandler(DiscordSocketClient client, IInteractionHolder interactionsHolder,
            IProvider<IInteractionMatcher> matcherProvider)
        {
            _interactionMatcherProvider = matcherProvider;
            _commandsTokenSource = new CancellationTokenSource();
            _interactionsHolder = interactionsHolder;
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
        /// <param name="interaction"></param>
        /// <returns></returns>
        protected virtual Task HandleInteractionCreated(SocketInteraction interaction)
        {
            HeldInteraction? heldInteraction =
                _interactionsHolder.TryMatch(_interactionMatcherProvider.GetData(), interaction);

            if (heldInteraction != null)
            {
                return heldInteraction.Executor.TryExecuteInteraction(
                    heldInteraction.Info,
                    interaction,
                    _commandsTokenSource.Token);
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