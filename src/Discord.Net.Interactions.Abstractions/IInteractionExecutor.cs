using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Discord.Net.Interactions.Abstractions
{
    /// <summary>
    /// Executor of a slash command interaction
    /// </summary>
    public interface IInteractionExecutor
    {
        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="info">Information about the slash command</param>
        /// <param name="interaction">Interaction</param>
        /// <param name="token">Cancel token</param>
        /// <returns></returns>
        public Task TryExecuteInteraction(InteractionInfo info, SocketInteraction interaction, CancellationToken token = default);
    }
}