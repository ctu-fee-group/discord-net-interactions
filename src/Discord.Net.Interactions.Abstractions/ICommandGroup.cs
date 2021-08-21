using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Interactions.Abstractions
{
    /// <summary>
    /// Group of commands exposing a function for registering the commands
    /// </summary>
    public interface ICommandGroup<TInteractionInfo>
        where TInteractionInfo : InteractionInfo
    {
        /// <summary>
        /// Builds and registers slash commands
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task SetupCommandsAsync(ICommandHolder<TInteractionInfo> holder, CancellationToken token = new CancellationToken());
    }
}