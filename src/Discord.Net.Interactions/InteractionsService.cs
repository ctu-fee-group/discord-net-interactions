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
    public class InteractionsService<TSlashInfo>
        where TSlashInfo : SlashCommandInfo
    {
        protected readonly InteractionHandler<TSlashInfo> _interactionHandler;
        protected readonly ICommandHolder<TSlashInfo> _commandHolder;
        protected readonly ICommandsRegistrator<TSlashInfo> _commandRegistrator;
        protected readonly ICommandsGroupProvider<TSlashInfo> _commandsGroupProvider;
        
        public InteractionsService(InteractionHandler<TSlashInfo> interactionHandler,
            ICommandHolder<TSlashInfo> commandHolder,
            ICommandsRegistrator<TSlashInfo> commandsRegistrator,
            ICommandsGroupProvider<TSlashInfo> commandsGroupProvider)
        {
            _interactionHandler = interactionHandler;
            _commandHolder = commandHolder;
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
                .Select(x => x.SetupCommandsAsync(_commandHolder, token)));

            await _commandRegistrator.RegisterCommandsAsync(_commandHolder, token);
            await _interactionHandler.StartAsync(token);
        }

        /// <summary>
        /// Refresh command permissions
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual Task RefreshAsync(CancellationToken token = new CancellationToken())
        {
            return _commandRegistrator.RefreshCommandsAndPermissionsAsync(_commandHolder, token);
        }

        /// <summary>
        /// Stops all current running handlers
        /// and unregister commands
        /// </summary>
        /// <param name="token"></param>
        public virtual async Task StopAsync(CancellationToken token = new CancellationToken())
        {
            await _commandRegistrator.UnregisterCommandsAsync(_commandHolder, token);
            await _interactionHandler.StopAsync(token);
        }
    }
}