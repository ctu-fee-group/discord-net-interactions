using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Discord.Net.Interactions.Abstractions
{
    public delegate Task CommandDelegate(SocketSlashCommand command, CancellationToken token);

    public delegate Task CommandDelegate<in T1>(SocketSlashCommand command, T1 arg1, CancellationToken token);

    public delegate Task CommandDelegate<in T1, in T2>(SocketSlashCommand command, T1 arg1, T2 arg2,
        CancellationToken token);

    public delegate Task CommandDelegate<in T1, in T2, in T3>(SocketSlashCommand command, T1 arg1, T2 arg2, T3 arg3,
        CancellationToken token);

    public delegate Task CommandDelegate<in T1, in T2, in T3, in T4>(SocketSlashCommand command, T1 arg1, T2 arg2,
        T3 arg3, T4 arg4, CancellationToken token);

    public delegate Task CommandDelegate<in T1, in T2, in T3, in T4, T5>(SocketSlashCommand command, T1 arg1, T2 arg2,
        T3 arg3, T4 arg4, T5 arg5, CancellationToken token);

    /// <summary>
    /// Creator of SlashCommandHandler different types may be used for subcommands or custom matching
    /// </summary>
    /// <typeparam name="TMatcherType">Type that will be passed to the matcher as argument</typeparam>
    /// <typeparam name="TDelegateType">Type of the delegate that will be called as the handler</typeparam>
    public interface ICommandHandlerCreator<TMatcherType, TDelegateType>
    {
        /// <summary>
        /// Creates SlashCommandHandler for given matches
        /// </summary>
        /// <param name="matchers">List of matchers that specify what function is matched given conditions</param>
        /// <returns></returns>
        public SlashCommandHandler CreateHandlerForCommand(
            IEnumerable<(Func<TMatcherType, bool>, TDelegateType)> matchers);

        //InstancedSlashCommandHandle

        /// <summary>
        /// Creates SlashCommandHandler for given matches
        /// </summary>
        /// <param name="matchers">List of matchers that specify what function is matched given conditions</param>
        /// <returns></returns>
        public InstancedSlashCommandHandler CreateInstancedHandlerForCommand(
            IEnumerable<(Func<TMatcherType, bool>, MethodInfo)> matchers);
    }
}