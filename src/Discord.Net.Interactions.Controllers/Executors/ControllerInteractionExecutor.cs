using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Discord.Net.Interactions.Controllers.Executors
{
    public class ControllerInteractionExecutor : IInteractionExecutor
    {
        private readonly IControllerResolver _resolver;
        private readonly ILogger _logger;
        private readonly IDiscordClient _client;
        private readonly IControllerConfigurator _configurator;

        public ControllerInteractionExecutor(
            IDiscordClient client,
            IControllerConfigurator configurator,
            IControllerResolver resolver,
            ILogger logger)
        {
            _configurator = configurator;
            _client = client;
            _logger = logger;
            _resolver = resolver;
        }

        public async Task TryExecuteInteraction(InteractionInfo info, SocketInteraction interaction,
            CancellationToken token = default)
        {
            try
            {
                if (info.InstancedHandler is null)
                {
                    throw new InvalidOperationException(
                        "Controller executor is able to execute instanced handlers only");
                }

                using IInteractionControllerInfo controllerInfo = _resolver.ResolveController(info);
                
                _configurator.FillControllerContext(controllerInfo, _client, interaction, info, token);

                _logger.LogInformation(
                    $@"Handling interaction {interaction.GetName()} executed by {interaction.GetUser()} in controller {controllerInfo.GetType().FullName}");
                await info.InstancedHandler(controllerInfo.Controller, interaction, token);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"There was an exception while executing controller interaction {interaction.GetName()} executed by user {interaction.GetUser()}");
            }
        }
    }
}