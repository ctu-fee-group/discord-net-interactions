using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Interactions.Abstractions
{
    /// <summary>
    /// Class used for registering commands
    /// </summary>
    public interface ICommandsRegistrator<TInteractionInfo>
        where TInteractionInfo : InteractionInfo
    {
        /// <summary>
        /// Registers commands with Discord
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task RegisterCommandsAsync(ICommandHolder<TInteractionInfo> holder, CancellationToken token = default);
        
        /// <summary>
        /// Unregisters all held commands with Discord
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task UnregisterCommandsAsync(ICommandHolder<TInteractionInfo> holder, CancellationToken token = default);

        /// <summary>
        /// Refreshes all held commands DefaultPermission and permissions
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task RefreshCommandsAndPermissionsAsync(ICommandHolder<TInteractionInfo> holder, CancellationToken token = default);
    }
}