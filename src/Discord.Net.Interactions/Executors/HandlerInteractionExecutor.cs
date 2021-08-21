using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Discord.Net.Interactions.Executors
{
    /// <summary>
    /// Basic CommandExecutor calling Handler of the command in try catch, logging if there was an error
    /// </summary>
    public class HandlerInteractionExecutor : IInteractionExecutor
    {
        private readonly ILogger _logger;
        
        public HandlerInteractionExecutor(ILogger logger)
        {
            _logger = logger;
        }
        
        public async Task TryExecuteInteraction(InteractionInfo info, SocketInteraction interaction, CancellationToken token = default)
        {
            try
            {
                if (info.Handler is null)
                {
                    throw new InvalidOperationException("HandlerCommandExecutor can handle only non-instanced commands");
                }
                
                _logger.LogInformation($@"Handling interaction {interaction.GetName()} executed by {interaction.GetUser()}");
                await info.Handler(interaction, token);
            }
            catch (OperationCanceledException)
            {
                token.ThrowIfCancellationRequested();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $@"Interaction handler for {interaction.GetName()} has thrown an exception");
            }
        }
    }
}