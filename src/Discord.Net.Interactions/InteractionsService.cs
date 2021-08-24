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
        protected readonly IInteractionHolder _interactionHolder;
        protected readonly ICommandsRegistrator _commandRegistrator;
        protected readonly IProvider<ICommandGroup> _commandsGroupProvider;
        
        public InteractionsService(InteractionHandler interactionHandler,
            IInteractionHolder interactionHolder,
            ICommandsRegistrator commandsRegistrator,
            IProvider<ICommandGroup> commandsGroupProvider)
        {
            _interactionHandler = interactionHandler;
            _interactionHolder = interactionHolder;
            _commandRegistrator = commandsRegistrator;
            _commandsGroupProvider = commandsGroupProvider;
        }

        /// <summary>
        /// Setup commands from groups, register commands, setup events
        /// </summary>
        /// <param name="registerCommands">Whether to register all commands at startup</param>
        /// <param name="token"></param>
        public virtual async Task StartAsync(bool registerCommands = true, CancellationToken token = new CancellationToken())
        {
            await Task.WhenAll(_commandsGroupProvider.GetData()
                .Select(x => x.SetupCommandsAsync(_interactionHolder, token)));

            await _commandRegistrator.RegisterCommandsAsync(_interactionHolder, token);
            await _interactionHandler.StartAsync(token);
        }

        /// <summary>
        /// Register commands to specified guild
        /// </summary>
        /// <param name="guildId">What guild to register commands to</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual Task RegisterGuildCommandsAsync(ulong guildId,
            CancellationToken token = new CancellationToken())
        {
            return _commandRegistrator.RegisterGuildCommandsAsync(guildId, _interactionHolder, token);
        }
        
        /// <summary>
        /// Unregister commands from specified guild
        /// </summary>
        /// <param name="guildId">What guild to unregister commands from</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual Task UnregisterGuildCommandsAsync(ulong guildId,
            CancellationToken token = new CancellationToken())
        {
            return _commandRegistrator.UnregisterGuildCommandsAsync(guildId, _interactionHolder, token);
        }

        /// <summary>
        /// Refresh command permissions
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual Task RefreshAsync(CancellationToken token = new CancellationToken())
        {
            return _commandRegistrator.RefreshCommandsAndPermissionsAsync(_interactionHolder, token);
        }

        /// <summary>
        /// Stops all current running handlers
        /// and unregister commands
        /// </summary>
        /// <param name="token"></param>
        public virtual async Task StopAsync(CancellationToken token = new CancellationToken())
        {
            await _commandRegistrator.UnregisterCommandsAsync(_interactionHolder, token);
            await _interactionHandler.StopAsync(token);
        }
    }
}