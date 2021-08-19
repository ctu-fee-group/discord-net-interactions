using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Discord.Net.Interactions.Executors
{
    /// <summary>
    /// Basic CommandExecutor calling Handlere of the command
    /// </summary>
    public class HandlerCommandExecutor<TSlashInfo> : ICommandExecutor<TSlashInfo>
        where TSlashInfo : SlashCommandInfo
    {
        private readonly ILogger _logger;
        
        public HandlerCommandExecutor(ILogger logger)
        {
            _logger = logger;
        }
        
        public async Task TryExecuteCommand(TSlashInfo info, SocketSlashCommand command, CancellationToken token = default)
        {
            try
            {
                _logger.LogInformation($@"Handling command /{command.Data.Name} executed by {command.User.Mention} ({command.User})");
                await info.Handler(command, token);
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