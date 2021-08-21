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
    /// <typeparam name="TInteractionInfo"></typeparam>
    public class InstancedInteractionExecutor : IInteractionExecutor
    {
        private readonly ILogger _logger;
        private Func<InteractionInfo, SocketInteraction, CancellationToken, object> _getInstance;
        
        /// <param name="logger">Logger to log errors with</param>
        /// <param name="getInstance">Obtain new instance for execution of the command</param>
        public InstancedInteractionExecutor(ILogger logger,
            Func<InteractionInfo, SocketInteraction, CancellationToken, object> getInstance)
        {
            _logger = logger;
            _getInstance = getInstance;
        }

        public async Task TryExecuteInteraction(InteractionInfo info, SocketInteraction interaction, CancellationToken token = default)
        {
            try
            {
                if (info.InstancedHandler is null)
                {
                    throw new InvalidOperationException("InstancedCommandExecutor can handle only instanced commands");
                }

                _logger.LogInformation(
                    $@"Handling command {interaction.GetName()} executed by {interaction.GetUser()}");
                
                object instance = _getInstance(info, interaction, token);
                await info.InstancedHandler(instance, interaction, token);
            }
            catch (OperationCanceledException)
            {
                token.ThrowIfCancellationRequested();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $@"Command handler for command {interaction.GetName()} has thrown an exception");
            }
        }
    }
}