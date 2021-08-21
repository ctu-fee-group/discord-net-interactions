using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Discord.Net.Interactions.Executors
{
    public class PermissionCheckCommandExecutor<TInteractionInfo> : ICommandExecutor<TInteractionInfo>
        where TInteractionInfo : InteractionInfo
    {
        private readonly ILogger _logger;
        private readonly ICommandExecutor<TInteractionInfo> _executor;
        private readonly ICommandPermissionsResolver<TInteractionInfo> _commandPermissionsResolver;

        public PermissionCheckCommandExecutor(ILogger logger,
            ICommandPermissionsResolver<TInteractionInfo> commandPermissionsResolver,
            ICommandExecutor<TInteractionInfo> underlyingExecutor)
        {
            _commandPermissionsResolver = commandPermissionsResolver;
            _executor = underlyingExecutor;
            _logger = logger;
        }

        public async Task TryExecuteInteraction(TInteractionInfo info, SocketInteraction interaction,
            CancellationToken token = default)
        {
            if (await _commandPermissionsResolver.HasPermissionAsync(interaction.User, info, token))
            {
                await _executor.TryExecuteInteraction(info, interaction, token);
            }
            else
            {
                _logger.LogWarning(
                    $@"User {interaction.GetUser()} tried to use interaction {interaction.GetName()}, but doesn't have permissions to do so");
            }
        }
    }
}