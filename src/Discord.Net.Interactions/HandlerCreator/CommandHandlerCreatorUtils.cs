using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.Net.Interactions.Reflection;
using Discord.WebSocket;

namespace Discord.Net.Interactions.HandlerCreator
{
    public class CommandHandlerCreatorUtils
    {

        /// <summary>
        /// Create SlashCommandHandler from given Delegate.
        /// It should have the following signature: SocketSlashCommand, *arguments for the command with matching names*, CancellationToken
        /// Uses <see cref="EfficientInvoker"/> for invoking the delegate faster.
        /// </summary>
        /// <param name="function">What function to call during handling</param>
        /// <param name="getArguments">Function to obtain arguments for the delegate with</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static InstancedSlashCommandHandler CreateHandler(EfficientInvoker invoker,
            Func<SocketSlashCommandData, IEnumerable<object?>?> getArguments)
        {
            return (instance, command, token) =>
            {
                List<object?> args = new() {command};
                args.AddRange(getArguments(command.Data) ?? Enumerable.Empty<object?>());
                args.Add(token);

                return Invoke(command.Data.Name, invoker, instance, args.ToArray());
            };
        }

        /// <summary>
        /// Create SlashCommandHandler from given Delegate.
        /// It should have the following signature: SocketSlashCommand, *arguments for the command with matching names*, CancellationToken
        /// Uses <see cref="EfficientInvoker"/> for invoking the delegate faster.
        /// </summary>
        /// <param name="function">What function to call during handling</param>
        /// <param name="getArguments">Function to obtain arguments for the delegate with</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Func<object, SocketSlashCommand, T, CancellationToken, Task> CreateHandler<T>(EfficientInvoker invoker,
            Func<SocketSlashCommandData, T, IEnumerable<object?>?> getArguments)
        {
            return (instance, command, helper, token) =>
            {
                List<object?> args = new() {command};
                args.AddRange(getArguments(command.Data, helper) ?? Enumerable.Empty<object?>());
                args.Add(token);

                return Invoke(command.Data.Name, invoker, instance, args.ToArray());
            };
        }

        public static IEnumerable<object?> GetParametersFromOptions(MethodInfo methodInfo,
            IEnumerable<SocketSlashCommandDataOption>? options)
        {
            ParameterInfo[] parameters = methodInfo.GetParameters();
            object?[] arguments = new object?[parameters.Length - 2];

            if (options == null)
            {
                return arguments;
            }

            IEnumerable<SocketSlashCommandDataOption> socketSlashCommandDataOptions =
                options as SocketSlashCommandDataOption[] ?? options.ToArray();
            for (int i = 0; i < arguments.Length; i++)
            {
                string name = parameters[i + 1].Name?.ToLower() ?? "";
                arguments[i] = socketSlashCommandDataOptions.FirstOrDefault(x => x.Name == name)?.Value;
            }

            return arguments;
        }

        private static Task Invoke(string name, EfficientInvoker invoker, object instance, object?[] args)
        {
            object? data = invoker.Invoke(instance, args.ToArray());

            if (data is null)
            {
                throw new InvalidOperationException($@"Command handler of {name} returned null instead of Task");
            }

            return (Task) data;
        }
    }
}