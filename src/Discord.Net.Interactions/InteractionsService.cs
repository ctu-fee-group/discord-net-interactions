using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.Net.Interactions.Handlers;

namespace Discord.Net.Interactions
{
    /// <summary>
    /// Service that will correctly initialize interactions handling along with initializing command groups
    /// </summary>
    public class InteractionsService
    {
        protected readonly InteractionHandler _interactionHandler;
        protected readonly IInteractionHolder InteractionHolder;
        protected readonly ICommandsRegistrator _commandRegistrator;
        protected readonly ICommandsGroupProvider _commandsGroupProvider;
        
        public InteractionsService(InteractionHandler interactionHandler,
            IInteractionHolder interactionHolder,
            ICommandsRegistrator commandsRegistrator,
            ICommandsGroupProvider commandsGroupProvider)
        {
            _interactionHandler = interactionHandler;
            InteractionHolder = interactionHolder;
            _commandRegistrator = commandsRegistrator;
            _commandsGroupProvider = commandsGroupProvider;
        }

        /// <summary>
        /// Setup commands from groups, register commands, setup events
        /// </summary>
        /// <param name="token"></param>
        public virtual async Task StartAsync(CancellationToken token = new CancellationToken())
        {
            await Task.WhenAll(_commandsGroupProvider.GetGroups()
                .Select(x => x.SetupCommandsAsync(InteractionHolder, token)));

            await _commandRegistrator.RegisterCommandsAsync(InteractionHolder, token);
            await _interactionHandler.StartAsync(token);
        }

        /// <summary>
        /// Refresh command permissions
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual Task RefreshAsync(CancellationToken token = new CancellationToken())
        {
            return _commandRegistrator.RefreshCommandsAndPermissionsAsync(InteractionHolder, token);
        }

        /// <summary>
        /// Stops all current running handlers
        /// and unregister commands
        /// </summary>
        /// <param name="token"></param>
        public virtual async Task StopAsync(CancellationToken token = new CancellationToken())
        {
            await _commandRegistrator.UnregisterCommandsAsync(InteractionHolder, token);
            await _interactionHandler.StopAsync(token);
        }
    }
}