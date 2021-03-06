using System;
using System.Linq;
using System.Reflection;

namespace Discord.Net.Interactions.Abstractions
{
    public static class ICommandHandlerCreatorExtensions
    {
        /// <summary>
        /// Passes one matcher that always return true to <see cref="ICommandHandlerCreator{T}.CreateHandlerForCommand"/>
        /// Creates SlashCommandHandler that will always execute given delegate
        /// </summary>
        /// <param name="creator"></param>
        /// <param name="deleg">Delegate to execute with the command</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static DiscordInteractionHandler CreateHandlerForCommand<T>(this ICommandHandlerCreator<T> creator,
            Delegate deleg)
        {
            return creator
                .CreateHandlerForCommand((_ => true, deleg));
        }

        /// <summary>
        /// Passes params to <see cref="ICommandHandlerCreator{T}.CreateHandlerForCommand"/>
        /// Creates SlashCommandHandler based on matchers
        /// </summary>
        /// <param name="creator"></param>
        /// <param name="matchers"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static DiscordInteractionHandler CreateHandlerForCommand<T>(this ICommandHandlerCreator<T> creator, params (Func<T, bool>, Delegate)[] matchers)
        {
            return creator.CreateHandlerForCommand(matchers.AsEnumerable());
        }

        /// <summary>
        /// Passes equals matchers to <see cref="ICommandHandlerCreator{T}.CreateHandlerForCommand"/>
        /// Creates SlashCommandHandler matching T objects in matchers
        /// </summary>
        /// <param name="creator"></param>
        /// <param name="matchers"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static DiscordInteractionHandler CreateHandlerForCommand<T>(this ICommandHandlerCreator<T> creator,
            params (T, Delegate)[] matchers)
        where T : notnull
        {
            return creator
                .CreateHandlerForCommand(matchers.Select(
                    x => ValueTuple.Create<Func<T, bool>, Delegate>(((y) => x.Item1.Equals(y)), x.Item2)));
        }
        
                /// <summary>
        /// Passes one matcher that always return true to <see cref="ICommandHandlerCreator{T}.CreateHandlerForCommand"/>
        /// Creates SlashCommandHandler that will always execute given delegate
        /// </summary>
        /// <param name="creator"></param>
        /// <param name="deleg">Delegate to execute with the command</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static InstancedDiscordInteractionHandler CreateInstancedHandlerForCommand<T>(this ICommandHandlerCreator<T> creator,
            MethodInfo methodInfo)
        {
            return creator
                .CreateInstancedHandlerForCommand((_ => true, methodInfo));
        }

        /// <summary>
        /// Passes params to <see cref="ICommandHandlerCreator{T}.CreateHandlerForCommand"/>
        /// Creates SlashCommandHandler based on matchers
        /// </summary>
        /// <param name="creator"></param>
        /// <param name="matchers"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static InstancedDiscordInteractionHandler CreateInstancedHandlerForCommand<T>(this ICommandHandlerCreator<T> creator, params (Func<T, bool>, MethodInfo)[] matchers)
        {
            return creator.CreateInstancedHandlerForCommand(matchers.AsEnumerable());
        }

        /// <summary>
        /// Passes equals matchers to <see cref="ICommandHandlerCreator{T}.CreateHandlerForCommand"/>
        /// Creates SlashCommandHandler matching T objects in matchers
        /// </summary>
        /// <param name="creator"></param>
        /// <param name="matchers"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static InstancedDiscordInteractionHandler CreateInstancedHandlerForCommand<T>(this ICommandHandlerCreator<T> creator,
            params (T, MethodInfo)[] matchers)
        where T : notnull
        {
            return creator
                .CreateInstancedHandlerForCommand(matchers.Select(
                    x => ValueTuple.Create<Func<T, bool>, MethodInfo>(((y) => x.Item1.Equals(y)), x.Item2)));
        }
    }
}