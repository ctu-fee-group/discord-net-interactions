using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.WebSocket;

namespace Discord.Net.Interactions.Executors
{
    /// <summary>
    /// Executor that will auto defer the command.
    /// If message is supplied, respond with that message will be sent.
    /// If message is null, Defer will be used.
    ///
    /// The message will be sent ephemerally.
    /// </summary>
    public class AutoDeferInteractionExecutor : IInteractionExecutor
    {
        private readonly IInteractionExecutor _executor;

        public AutoDeferInteractionExecutor(IInteractionExecutor underlyingExecutor, string? message = "I am thinking...")
        {
            _executor = underlyingExecutor;
            Message = message;
        }

        /// <summary>
        /// Message to send. If null, Defer
        /// </summary>
        public string? Message { get; set; }

        public async Task TryExecuteInteraction(InteractionInfo info, SocketInteraction interaction,
            CancellationToken token = default)
        {
            if (Message is null)
            {
                await interaction
                    .DeferAsync(ephemeral: true, options: new RequestOptions() {CancelToken = token});
            }
            else
            {
                await interaction
                    .RespondAsync(Message, ephemeral: true, options: new RequestOptions() {CancelToken = token});
            }

            await _executor.TryExecuteInteraction(info, interaction, token);
        }
    }
}