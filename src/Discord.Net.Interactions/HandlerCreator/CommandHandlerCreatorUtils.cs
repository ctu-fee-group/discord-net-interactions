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
        /// Create SlashCommandHandler from given EfficientInvoker.
        /// It should have the following signature: SocketSlashCommand, *arguments for the command with matching names*, CancellationToken.
        /// Uses <see cref="EfficientInvoker"/> for invoking the delegate faster.
        /// </summary>
        /// <param name="invoker">What invoker will be used to invoke the command</param>
        /// <param name="getArguments">Function to obtain arguments for the invoker</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static InstancedDiscordInteractionHandler CreateHandler(EfficientInvoker invoker,
            Func<SocketSlashCommandData, IEnumerable<object?>?> getArguments)
        {
            return (instance, interaction, token) =>
            {
                if (interaction is not SocketSlashCommand command)
                {
                    throw new InvalidOperationException("HandlerCreators can be used only for slash commands");
                }
                
                List<object?> args = new() {command};
                args.AddRange(getArguments(command.Data) ?? Enumerable.Empty<object?>());
                args.Add(token);

                return Invoke(command.Data.Name, invoker, instance, args.ToArray());
            };
        }

        /// <summary>
        /// Create SlashCommandHandler from given EfficientInvoker.
        /// It should have the following signature: SocketSlashCommand, *arguments for the command with matching names*, CancellationToken
        /// Uses <see cref="EfficientInvoker"/> for invoking the delegate faster.
        /// </summary>
        /// <remarks>
        /// This function is different from non-generic CreateHandler, because getArguments function accepts generic helper argument
        /// that can be used for storing anything important for retrieving arguments
        /// </remarks>
        /// <param name="invoker">What invoker will be used to invoke the command</param>
        /// <param name="getArguments">Function to obtain arguments for the delegate with</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Func<object, IDiscordInteraction, T, CancellationToken, Task> CreateHandler<T>(EfficientInvoker invoker,
            Func<SocketSlashCommandData, T, IEnumerable<object?>?> getArguments)
        {
            return (instance, interaction, helper, token) =>
            {
                if (interaction is not SocketSlashCommand command)
                {
                    throw new InvalidOperationException("HandlerCreators can be used only for slash commands");
                }
                
                List<object?> args = new() {command};
                args.AddRange(getArguments(command.Data, helper) ?? Enumerable.Empty<object?>());
                args.Add(token);

                return Invoke(command.Data.Name, invoker, instance, args.ToArray());
            };
        }

        /// <summary>
        /// Matches positions of options of command to method parameters.
        /// Parameters will be matched to the names of the options of the command.
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="options"></param>
        /// <returns></returns>
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