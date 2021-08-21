using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.Net.Interactions.Reflection;
using Discord.WebSocket;

namespace Discord.Net.Interactions.HandlerCreator
{
    /// <summary>
    /// Creates SlashCommandHandler for command with one or more level subcommands.
    /// Matchers should match against name of all of the subcommands
    ///
    /// So for example, if we have /command subcmd subsubcmd, then
    /// matcher for "subcmd subsubcmd" should be present
    /// </summary>
    public class SubCommandHandlerCreator : ICommandHandlerCreator<string>
    {
        private record HandlerMatcher<T>(Func<string, bool> Matcher,
            Func<Delegate, SocketSlashCommand, SocketSlashCommandDataOption?, CancellationToken, Task> Handler,
            T Instance);

        public DiscordInteractionHandler CreateHandlerForCommand(IEnumerable<(Func<string, bool>, Delegate)> matchers) =>
            GetCommandHandler(ParseMatchers<Delegate>(matchers, EfficientInvoker.ForDelegate, x => x.Method));

        public InstancedDiscordInteractionHandler CreateInstancedHandlerForCommand(
            IEnumerable<(Func<string, bool>, MethodInfo)> matchers) =>
            GetInstancedCommandHandler(ParseMatchers<MethodInfo>(matchers,
                EfficientInvoker.ForMethod, x => x));
        
        private HandlerMatcher<T> GetMatchedHandler<T>(List<HandlerMatcher<T>> matchers, SocketSlashCommand command,
            out SocketSlashCommandDataOption? outOption, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            string matchAgainst = GetSubcommandArguments(command, out SocketSlashCommandDataOption? option);
            HandlerMatcher<T>? handler = matchers.FirstOrDefault(x => x.Matcher(matchAgainst));

            outOption = option;

            if (handler is null)
            {
                throw new InvalidOperationException(
                    $"Could not match subcommand /{command.Data.Name} {matchAgainst}");
            }

            return handler;
        }
        
        private List<HandlerMatcher<T>> ParseMatchers<T>(IEnumerable<(Func<string, bool>, T)> matchers,
            Func<T, EfficientInvoker> getInvoker, Func<T, MethodInfo> getMethodInfo)
        {
            return matchers
                .Select(x =>
                {
                    EfficientInvoker invoker = getInvoker(x.Item2);
                    return new HandlerMatcher<T>(
                        x.Item1,
                        CommandHandlerCreatorUtils.CreateHandler<SocketSlashCommandDataOption?>(invoker,
                            (data, option) =>
                                CommandHandlerCreatorUtils.GetParametersFromOptions(getMethodInfo(x.Item2), option?.Options)),
                        x.Item2
                    );
                })
                .ToList();
        }


        private InstancedDiscordInteractionHandler GetInstancedCommandHandler(List<HandlerMatcher<MethodInfo>> matchers)
        {
            return (instance, interaction, token) =>
            {
                if (interaction is not SocketSlashCommand command)
                {
                    throw new InvalidOperationException("HandlerCreators can be used only for slash commands");
                }
                
                HandlerMatcher<MethodInfo> matcher =
                    GetMatchedHandler(matchers, command, out SocketSlashCommandDataOption? option, token);
                Delegate methodDelegate = matcher.Instance.CreateDelegate<Delegate>(instance);
                token.ThrowIfCancellationRequested();

                return
                    matcher.Handler(methodDelegate, command, option, token);
            };
        }

        private DiscordInteractionHandler GetCommandHandler(List<HandlerMatcher<Delegate>> matchers)
        {
            return (interaction, token) =>
            {
                if (interaction is not SocketSlashCommand command)
                {
                    throw new InvalidOperationException("HandlerCreators can be used only for slash commands");
                }
                
                token.ThrowIfCancellationRequested();

                HandlerMatcher<Delegate> matchedHandler =
                    GetMatchedHandler(matchers, command, out SocketSlashCommandDataOption? option, token);
                return matchedHandler.Handler(matchedHandler.Instance, command, option, token);
            };
        }

        private string GetSubcommandArguments(SocketSlashCommand command,
            out SocketSlashCommandDataOption? currentOption)
        {
            List<string> subcommands = new();
            SocketSlashCommandDataOption? nextOption = command.Data.Options.FirstOrDefault();
            currentOption = null;

            IReadOnlyCollection<SocketSlashCommandDataOption>? options = command.Data.Options;

            while (nextOption?.Type is (ApplicationCommandOptionType.SubCommandGroup or ApplicationCommandOptionType
                .SubCommand))
            {
                subcommands.Add(nextOption.Name);

                currentOption = nextOption;
                options = options?.First().Options;
                nextOption = options?.FirstOrDefault();
            }

            return string.Join(' ', subcommands);
        }
    }
}