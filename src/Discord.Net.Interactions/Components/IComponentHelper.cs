using System;
using Discord.Net.Interactions.Abstractions;
using Discord.Net.Interactions.Executors;

namespace Discord.Net.Interactions.Components
{
    public interface IComponentHelper
    {
        /// <summary>
        /// Handle specified message components based on custom ids
        /// </summary>
        /// <param name="message">What message should the handlers react</param>
        /// <param name="user">The user to accept from (if set, no other user will be accepted)</param>
        /// <param name="builderAction">Action to configure executor builder</param>
        /// <param name="infos">What handlers to add (custom id, handler)</param>
        public void ForMessage(IMessage message, IUser? user,
            Action<InteractionExecutorBuilder<MessageComponentInfo>> builderAction,
            params (string, DiscordInteractionHandler)[] infos);

        /// <summary>
        /// Handle specified message components based on custom ids,
        /// the infos should be treated as choices, thus react only once
        /// and only one choice is accepted
        /// </summary>
        /// <param name="message">What message should the handlers react</param>
        /// <param name="user">The user to accept from (if set, no other user will be accepted)</param>
        /// <param name="builderAction">Action to configure executor builder</param>
        /// <param name="infos">What handlers to add (custom id, handler)</param>
        public void ForMessageChoice(IMessage message, IUser? user,
            Action<InteractionExecutorBuilder<MessageComponentInfo>> builderAction,
            params (string, DiscordInteractionHandler)[] infos);
    }
}