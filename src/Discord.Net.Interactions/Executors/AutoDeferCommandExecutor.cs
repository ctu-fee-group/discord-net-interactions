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
    public class AutoDeferCommandExecutor<TSlashInfo> : ICommandExecutor<TSlashInfo>
        where TSlashInfo : SlashCommandInfo
    {
        private readonly ICommandExecutor<TSlashInfo> _executor;

        public AutoDeferCommandExecutor(ICommandExecutor<TSlashInfo> underlyingExecutor, string? message = "I am thinking...")
        {
            _executor = underlyingExecutor;
            Message = message;
        }

        /// <summary>
        /// Message to send. If null, Defer
        /// </summary>
        public string? Message { get; set; }

        public async Task TryExecuteCommand(TSlashInfo info, SocketSlashCommand command,
            CancellationToken token = default)
        {
            if (Message is null)
            {
                await command
                    .DeferAsync(ephemeral: true, options: new RequestOptions() {CancelToken = token});
            }
            else
            {
                await command
                    .RespondAsync(Message, ephemeral: true, options: new RequestOptions() {CancelToken = token});
            }

            await _executor.TryExecuteCommand(info, command, token);
        }
    }
}