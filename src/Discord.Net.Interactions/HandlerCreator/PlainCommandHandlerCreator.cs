using System;
using System.Collections.Generic;
using System.Linq;
using Discord.Net.Interactions.Abstractions;

namespace Discord.Net.Interactions.HandlerCreator
{
    /// <summary>
    /// Creates SlashCommandHandler for command without subcommands.
    /// It should receive only 1 matcher and that should always match
    /// </summary>
    public class PlainCommandHandlerCreator : ICommandHandlerCreator<string, Delegate>
    {
        public SlashCommandHandler CreateHandlerForCommand(IEnumerable<(Func<string, bool>, Delegate)> matchers)
        {
            var valueTuples = matchers as (Func<string, bool>, Delegate)[] ?? matchers.ToArray();
            if (valueTuples.Count() != 1)
            {
                throw new InvalidOperationException(
                    "PlainCommandHandlerCreator can handle only one matcher that is always true");
            }

            var matcher = valueTuples.FirstOrDefault();
            return GetHandler(CommandHandlerCreatorUtils.CreateHandler(matcher.Item2,
                (data => CommandHandlerCreatorUtils.GetParametersFromOptions(matcher.Item2, data.Options))));
        }

        private SlashCommandHandler GetHandler(SlashCommandHandler handler)
        {
            return (command, token) =>
            {
                token.ThrowIfCancellationRequested();
                return handler(command, token);
            };
        }
    }
}