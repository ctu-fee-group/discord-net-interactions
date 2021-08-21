using System.Collections.Generic;

namespace Discord.Net.Interactions.Abstractions
{
    /// <summary>
    /// Holds SlashCommand information, so it is known what executor to execute
    /// </summary>
    /// <param name="Info"></param>
    /// <param name="Executor"></param>
    public record HeldInteraction(InteractionInfo Info, IInteractionExecutor Executor);
    /// <summary>
    /// Interface supporting registration and holding of slash commands
    /// </summary>
    public interface IInteractionHolder
    {
        public IEnumerable<HeldInteraction> Interactions { get; }

        /// <summary>
        /// Tries to get a slash command in list of commands by its name
        /// If command is not found, null is returned
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Slash command if it was found, otherwise null</returns>
        public HeldInteraction? TryMatch(IEnumerable<IInteractionMatcher> matchers, IDiscordInteraction interaction);
        
        /// <summary>
        /// Save command to collection
        /// </summary>
        /// <param name="info"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        public void AddInteraction(InteractionInfo info, IInteractionExecutor executor);
        
        /// <summary>
        /// Remove interaction
        /// </summary>
        /// <param name="info"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        public void RemoveInteraction(InteractionInfo info);
        
        /// <summary>
        /// Remove all commands from collection
        /// </summary>
        /// <param name="token"></param>
        public void RemoveCommands();
    }
}