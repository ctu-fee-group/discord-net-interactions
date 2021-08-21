using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.WebSocket;

namespace Discord.Net.Interactions.Executors
{
    /// <summary>
    /// Executes interaction once and then removes it from the holder so it won't be executed again
    /// </summary>
    public class OnlyOnceInteractionExecutor : IInteractionExecutor
    {
        private readonly IInteractionHolder _holder;
        private readonly IInteractionExecutor _underlyingExecutor;
        
        public OnlyOnceInteractionExecutor(IInteractionHolder holder, IInteractionExecutor underlyingExecutor)
        {
            _underlyingExecutor = underlyingExecutor;
            _holder = holder;
        }
        
        public Task TryExecuteInteraction(InteractionInfo info, SocketInteraction interaction, CancellationToken token = default)
        {
            _holder.RemoveInteraction(info);
            return _underlyingExecutor.TryExecuteInteraction(info, interaction, token);
        }
    }
}