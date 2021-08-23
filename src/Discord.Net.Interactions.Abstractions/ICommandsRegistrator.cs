using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Interactions.Abstractions
{
    /// <summary>
    /// Class used for registering commands
    /// </summary>
    public interface ICommandsRegistrator
    {
        /// <summary>
        /// Registers all commands with Discord
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task RegisterCommandsAsync(IInteractionHolder holder, CancellationToken token = default);

        /// <summary>
        /// Registers commands with Discord only for one guild
        /// </summary>
        /// <param name="guildId">Guild id to register commands to</param>
        /// <param name="holder"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task RegisterGuildCommandsAsync(ulong guildId, IInteractionHolder holder, CancellationToken token = default);
        
        /// <summary>
        /// Unregisters all held commands with Discord
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task UnregisterCommandsAsync(IInteractionHolder holder, CancellationToken token = default);

        /// <summary>
        /// Unregisters held commands of specified guild with Discord
        /// </summary>
        /// <param name="guildId">Guild id to unregister commands from</param>
        /// <param name="holder"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task UnregisterGuildCommandsAsync(ulong guildId, IInteractionHolder holder, CancellationToken token = default);
        
        /// <summary>
        /// Refreshes held commands of specified guild DefaultPermission and permissions
        /// </summary>
        /// <param name="guildId"></param>
        /// <param name="holder"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task RefreshGuildCommandsAndPermissionsAsync(ulong guildId, IInteractionHolder holder, CancellationToken token = default);

        /// <summary>
        /// Refreshes all held commands DefaultPermission and permissions
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task RefreshCommandsAndPermissionsAsync(IInteractionHolder holder, CancellationToken token = default);
    }
}