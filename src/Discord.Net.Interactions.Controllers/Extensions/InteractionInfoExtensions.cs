using System;
using Discord.Net.Interactions.Abstractions;

namespace Discord.Net.Interactions.Controllers.Extensions
{
    public static class InteractionInfoExtensions
    {
        /// <summary>
        /// Cast InteractionInfo to specified type, if it is not that type, throw an exception
        /// </summary>
        /// <param name="interactionInfo"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">If the interaction info is not of the specified type</exception>
        public static T As<T>(this InteractionInfo interactionInfo)
            where T : InteractionInfo
        {
            if (interactionInfo is not T casted)
            {
                throw new InvalidOperationException(
                    $"Interaction info is not of type {typeof(T).FullName}. It is {interactionInfo.GetType().FullName}");
            }

            return casted;
        }

        /// <summary>
        /// Cast InteractionInfo to SlashCommandInfo. If it is not SlashCommandInfo, throw exception
        /// </summary>
        /// <param name="interactionInfo"></param>
        /// <returns></returns>
        public static SlashCommandInfo AsCommand(this InteractionInfo interactionInfo) =>
            interactionInfo.As<SlashCommandInfo>();
        
        /// <summary>
        /// Cast InteractionInfo to MessageComponentInfo. If it is not MessageComponentInfo, throw exception
        /// </summary>
        /// <param name="interactionInfo"></param>
        /// <returns></returns>
        public static MessageComponentInfo AsComponent(this InteractionInfo interactionInfo) =>
            interactionInfo.As<MessageComponentInfo>();
    }
}