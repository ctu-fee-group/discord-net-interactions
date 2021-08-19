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
    public class ThreadPoolCommandExecutor<TSlashInfo> : ICommandExecutor<TSlashInfo>
        where TSlashInfo : SlashCommandInfo
    {
        private readonly ILogger _logger;
        private readonly ICommandExecutor<TSlashInfo> _executor;

        public ThreadPoolCommandExecutor(ILogger logger, ICommandExecutor<TSlashInfo> underlyingExecutor)
        {
            _executor = underlyingExecutor;
            _logger = logger;
        }
        
        public Task TryExecuteCommand(TSlashInfo info, SocketSlashCommand command, CancellationToken token = default)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await _executor.TryExecuteCommand(info, command, token);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    _logger.LogError(0, e, "Command handler has thrown an exception while running in thread");
                }
            });
            
            return Task.CompletedTask;
        }
    }
}