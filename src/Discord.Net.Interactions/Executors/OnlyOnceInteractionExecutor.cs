using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Discord.Net.Interactions.Executors
{
    /// <summary>
    /// Executes interaction once and then removes it from the holder so it won't be executed again
    /// </summary>
    public class OnlyOnceInteractionExecutor : IInteractionExecutor
    {
        private readonly IInteractionHolder _holder;
        private readonly IInteractionExecutor _underlyingExecutor;
        private readonly object _firstRunLock = new ();
        private bool _alreadyRan = false;

        public OnlyOnceInteractionExecutor(IInteractionHolder holder, IInteractionExecutor underlyingExecutor)
        {
            _underlyingExecutor = underlyingExecutor;
            _holder = holder;
        }
        
        public Task TryExecuteInteraction(InteractionInfo info, SocketInteraction interaction, CancellationToken token = default)
        {
            bool execute = true;
            lock (_firstRunLock)
            {
                if (_alreadyRan)
                {
                    execute = false;
                }
                
                _alreadyRan = true;
            }

            if (execute)
            {
                _holder.RemoveInteraction(info);
                return _underlyingExecutor.TryExecuteInteraction(info, interaction, token);
            }
            else
            {
                return Task.CompletedTask;
            }
        }
    }
}