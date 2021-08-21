using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Discord.Net.Interactions.Executors
{
    /// <summary>
    /// Execute InstancedHandler of slash command info.
    /// Delegate to obtain instance with will be passed to the constructor.
    /// </summary>
    /// <typeparam name="TSlashInfo"></typeparam>
    public class InstancedCommandExecutor<TSlashInfo> : ICommandExecutor<TSlashInfo>
        where TSlashInfo : SlashCommandInfo
    {
        private readonly ILogger _logger;
        private Func<SlashCommandInfo, SocketSlashCommand, CancellationToken, object> _getInstance;
        
        /// <param name="logger">Logger to log errors with</param>
        /// <param name="getInstance">Obtain new instance for execution of the command</param>
        public InstancedCommandExecutor(ILogger logger,
            Func<SlashCommandInfo, SocketSlashCommand, CancellationToken, object> getInstance)
        {
            _logger = logger;
            _getInstance = getInstance;
        }

        public async Task TryExecuteCommand(TSlashInfo info, SocketSlashCommand command, CancellationToken token = default)
        {
            try
            {
                if (info.InstancedHandler is null)
                {
                    throw new InvalidOperationException("InstancedCommandExecutor can handle only instanced commands");
                }

                _logger.LogInformation(
                    $@"Handling command /{command.Data.Name} executed by {command.User.Mention} ({command.User})");
                
                object instance = _getInstance(info, command, token);
                await info.InstancedHandler(instance, command, token);
            }
            catch (OperationCanceledException)
            {
                token.ThrowIfCancellationRequested();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $@"Command handler for command /{command.Data.Name} has thrown an exception");
            }
        }
    }
}