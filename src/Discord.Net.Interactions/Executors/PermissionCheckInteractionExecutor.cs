using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Discord.Net.Interactions.Executors
{
    public class PermissionCheckInteractionExecutor<TInteractionInfo> : IInteractionExecutor
        where TInteractionInfo : InteractionInfo
    {
        private readonly ILogger _logger;
        private readonly IInteractionExecutor _executor;
        private readonly ICommandPermissionsResolver<TInteractionInfo> _commandPermissionsResolver;

        public PermissionCheckInteractionExecutor(ILogger logger,
            ICommandPermissionsResolver<TInteractionInfo> commandPermissionsResolver,
            IInteractionExecutor underlyingExecutor)
        {
            _commandPermissionsResolver = commandPermissionsResolver;
            _executor = underlyingExecutor;
            _logger = logger;
        }

        public async Task TryExecuteInteraction(InteractionInfo info, SocketInteraction interaction,
            CancellationToken token = default)
        {
            if (info is not TInteractionInfo typedInfo)
            {
                throw new InvalidOperationException("Cannot cast InteractionInfo to correct type for permission check");
            }
            
            if (await _commandPermissionsResolver.HasPermissionAsync(interaction.User, typedInfo, token))
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