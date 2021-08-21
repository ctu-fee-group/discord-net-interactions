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
        /// Registers commands with Discord
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task RegisterCommandsAsync(IInteractionHolder holder, CancellationToken token = default);
        
        /// <summary>
        /// Unregisters all held commands with Discord
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task UnregisterCommandsAsync(IInteractionHolder holder, CancellationToken token = default);

        /// <summary>
        /// Refreshes all held commands DefaultPermission and permissions
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task RefreshCommandsAndPermissionsAsync(IInteractionHolder holder, CancellationToken token = default);
    }
}