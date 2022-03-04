using System.Threading;
using Discord.Net.Interactions.Abstractions;
using Discord.WebSocket;

namespace Discord.Net.Interactions.Controllers
{
    public interface IInteractionState
    {
        /// <summary>
        /// What user executed the interaction
        /// </summary>
        public IUser User { get; }
        
        /// <summary>
        /// What channel was the interaction executed in
        /// May be null for user interactions
        /// </summary>
        public IChannel? Channel { get; }
        
        /// <summary>
        /// What guild was the interaction executed in,
        /// if it was executed in guild. Otherwise null
        /// </summary>
        public IGuild? Guild { get; }
        
        /// <summary>
        /// Stored info about the interaction
        /// </summary>
        public InteractionInfo InteractionInfo { get; }
        
        /// <summary>
        /// Interaction that was executed
        /// </summary>
        //public IDiscordInteraction Interaction { get; }
        public SocketInteraction Interaction { get; } // TODO: change to generic one on 3.0.0 release of Discord.NET labs
        
        /// <summary>
        /// Operation cancellation token
        /// </summary>
        public CancellationToken CancellationToken { get; }
    }
}