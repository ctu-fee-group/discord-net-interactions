using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.Net.Interactions.Reflection;

namespace Discord.Net.Interactions.HandlerCreator
{
    /// <summary>
    /// Creates SlashCommandHandler for command without subcommands.
    /// It should receive only 1 matcher and that should always match
    /// </summary>
    public class PlainCommandHandlerCreator : ICommandHandlerCreator<string>
    {
        public DiscordInteractionHandler CreateHandlerForCommand(IEnumerable<(Func<string, bool>, Delegate)> matchers)
        {
            var valueTuples = matchers as (Func<string, bool>, Delegate)[] ?? matchers.ToArray();

            Delegate matchedDelegate = GetMatched<Delegate>(valueTuples);
            InstancedDiscordInteractionHandler instancedHandler = CommandHandlerCreatorUtils.CreateHandler(
                EfficientInvoker.ForDelegate(matchedDelegate),
                (data => CommandHandlerCreatorUtils.GetParametersFromOptions(matchedDelegate.Method, data.Options)));

            return (command, token) => instancedHandler(matchedDelegate, command, token);
        }

        public InstancedDiscordInteractionHandler CreateInstancedHandlerForCommand(
            IEnumerable<(Func<string, bool>, MethodInfo)> matchers)
        {
            var valueTuples = matchers as (Func<string, bool>, MethodInfo)[] ?? matchers.ToArray();
            MethodInfo matchedMethod = GetMatched<MethodInfo>(valueTuples);
            EfficientInvoker invoker = EfficientInvoker.ForMethod(matchedMethod);

            return GetHandler(CommandHandlerCreatorUtils.CreateHandler(invoker,
                (data => CommandHandlerCreatorUtils.GetParametersFromOptions(matchedMethod, data.Options))));
        }

        private T GetMatched<T>((Func<string, bool>, T)[] valueTuples)
        {
            if (valueTuples.Length != 1)
            {
                throw new InvalidOperationException(
                    "PlainCommandHandlerCreator can handle only one matcher that is always true");
            }

            return valueTuples.FirstOrDefault().Item2;
        }

        private InstancedDiscordInteractionHandler GetHandler(InstancedDiscordInteractionHandler handler)
        {
            return (instance, command, token) =>
            {
                token.ThrowIfCancellationRequested();
                return handler(instance, command, token);
            };
        }
    }
}