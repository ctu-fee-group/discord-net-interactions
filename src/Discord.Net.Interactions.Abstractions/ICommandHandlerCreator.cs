using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Discord.Net.Interactions.Abstractions
{
    public delegate Task CommandDelegate(SocketInteraction interaction, CancellationToken token);

    public delegate Task CommandDelegate<in T1>(SocketInteraction interaction, T1 arg1, CancellationToken token);

    public delegate Task CommandDelegate<in T1, in T2>(SocketInteraction interaction, T1 arg1, T2 arg2,
        CancellationToken token);

    public delegate Task CommandDelegate<in T1, in T2, in T3>(SocketInteraction interaction, T1 arg1, T2 arg2, T3 arg3,
        CancellationToken token);

    public delegate Task CommandDelegate<in T1, in T2, in T3, in T4>(SocketInteraction interaction, T1 arg1, T2 arg2,
        T3 arg3, T4 arg4, CancellationToken token);

    public delegate Task CommandDelegate<in T1, in T2, in T3, in T4, T5>(SocketInteraction interaction, T1 arg1, T2 arg2,
        T3 arg3, T4 arg4, T5 arg5, CancellationToken token);

    /// <summary>
    /// Creator of SlashCommandHandler different types may be used for subcommands or custom matching
    /// </summary>
    /// <typeparam name="TMatcherType">Type that will be passed to the matcher as argument</typeparam>
    public interface ICommandHandlerCreator<TMatcherType>
    {
        /// <summary>
        /// Creates SlashCommandHandler for given matches
        /// </summary>
        /// <param name="matchers">List of matchers that specify what function is matched given conditions</param>
        /// <returns></returns>
        public DiscordInteractionHandler CreateHandlerForCommand(
            IEnumerable<(Func<TMatcherType, bool>, Delegate)> matchers);

        //InstancedSlashCommandHandle

        /// <summary>
        /// Creates InstancedSlashCommandHandler for given matches
        /// </summary>
        /// <remarks>
        /// Instanced slash command handler can be used to invoke command with different class instance
        /// every time
        /// </remarks>
        /// <param name="matchers">List of matchers that specify what function is matched given conditions</param>
        /// <returns></returns>
        public InstancedDiscordInteractionHandler CreateInstancedHandlerForCommand(
            IEnumerable<(Func<TMatcherType, bool>, MethodInfo)> matchers);
    }
}