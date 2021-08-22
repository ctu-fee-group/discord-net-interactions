using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.WebSocket;

namespace Discord.Net.Interactions.Executors
{
    public class RemoveInteractionsExecutor : IInteractionExecutor
    {
        private readonly IInteractionHolder _holder;
        private readonly InteractionInfo[] _removeInteractions;
        private readonly IInteractionExecutor _underlyingExecutor;

        public RemoveInteractionsExecutor(IInteractionHolder holder, InteractionInfo[] removeInteractions,
            IInteractionExecutor underlyingExecutor)
        {
            _holder = holder;
            _removeInteractions = removeInteractions;
            _underlyingExecutor = underlyingExecutor;
        }

        public Task TryExecuteInteraction(InteractionInfo info, SocketInteraction interaction,
            CancellationToken token = default)
        {
            foreach (InteractionInfo removeInteraction in _removeInteractions)
            {
                _holder.RemoveInteraction(removeInteraction);
            }

            return _underlyingExecutor.TryExecuteInteraction(info, interaction, token);
        }
    }
}