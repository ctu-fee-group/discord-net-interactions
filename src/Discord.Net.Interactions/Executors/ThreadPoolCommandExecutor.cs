using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Discord.Net.Interactions.Executors
{
    /// <summary>
    /// Command executor decorator for executing the command in separate thread
    /// If there is exception inside the thread, it will be logged
    /// </summary>
    public class ThreadPoolCommandExecutor<TInteractionInfo> : ICommandExecutor<TInteractionInfo>
        where TInteractionInfo : InteractionInfo
    {
        private readonly ILogger _logger;
        private readonly ICommandExecutor<TInteractionInfo> _executor;

        public ThreadPoolCommandExecutor(ILogger logger, ICommandExecutor<TInteractionInfo> underlyingExecutor)
        {
            _executor = underlyingExecutor;
            _logger = logger;
        }
        
        public Task TryExecuteInteraction(TInteractionInfo info, SocketInteraction interaction, CancellationToken token = default)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await _executor.TryExecuteInteraction(info, interaction, token);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    _logger.LogError(0, e, $"Interaction handler of {interaction.GetName()} has thrown an exception while running in thread");
                }
            });
            
            return Task.CompletedTask;
        }
    }
}