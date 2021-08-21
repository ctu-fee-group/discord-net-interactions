using System.Collections.Generic;

namespace Discord.Net.Interactions.Abstractions
{
    /// <summary>
    /// Holds SlashCommand information, so it is known what executor to execute
    /// </summary>
    /// <param name="Info"></param>
    /// <param name="Executor"></param>
    public record HeldInteraction<TInteractionInfo>(TInteractionInfo Info, ICommandExecutor<TInteractionInfo> Executor)
        where TInteractionInfo : InteractionInfo;
    
    /// <summary>
    /// Interface supporting registration and holding of slash commands
    /// </summary>
    public interface ICommandHolder<TInteractionInfo>
        where TInteractionInfo : InteractionInfo
    {
        public IEnumerable<HeldInteraction<TInteractionInfo>> Interactions { get; }

        /// <summary>
        /// Tries to get a slash command in list of commands by its name
        /// If command is not found, null is returned
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Slash command if it was found, otherwise null</returns>
        public HeldInteraction<TInteractionInfo>? TryMatch(IEnumerable<IInteractionMatcher> matchers, IDiscordInteraction interaction);
        
        /// <summary>
        /// Save command to collection
        /// </summary>
        /// <param name="info"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        public TInteractionInfo AddCommand(TInteractionInfo info, ICommandExecutor<TInteractionInfo> executor);
        
        /// <summary>
        /// Remove all commands from collection
        /// </summary>
        /// <param name="token"></param>
        public void RemoveCommands();
    }
}