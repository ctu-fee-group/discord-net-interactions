using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Discord.Net.Interactions.Executors
{
    public class PermissionCheckCommandExecutor : ICommandExecutor
    {
        private readonly ILogger _logger;
        private readonly ICommandExecutor _executor;
        private readonly ICommandPermissionsResolver<SlashCommandInfo> _commandPermissionsResolver;

        public PermissionCheckCommandExecutor(ILogger logger, ICommandPermissionsResolver<SlashCommandInfo> commandPermissionsResolver, ICommandExecutor underlyingExecutor)
        {
            _commandPermissionsResolver = commandPermissionsResolver;
            _executor = underlyingExecutor;
            _logger = logger;
        }

        public async Task TryExecuteCommand(SlashCommandInfo info, SocketSlashCommand command,
            CancellationToken token = default)
        {
            if (await _commandPermissionsResolver.HasPermissionAsync(command.User, info, token))
            {
                await _executor.TryExecuteCommand(info, command, token);
            }
            else
            {
                _logger.LogWarning(
                    $@"User {command.User.Mention} ({command.User}) tried to use command /{command.Data.Name}, but doesn't have permissions to do so");
            }
        }
    }
}