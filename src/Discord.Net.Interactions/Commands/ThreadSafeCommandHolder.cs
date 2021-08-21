using System.Collections.Generic;
using System.Linq;
using Discord.Net.Interactions.Abstractions;

namespace Discord.Net.Interactions.Commands
{
    /// <summary>
    /// Thread-safe implementation of CommandHolder using locks to achieve thread safety
    /// </summary>
    public class ThreadSafeCommandHolder<TInteractionInfo> : ICommandHolder<TInteractionInfo>
        where TInteractionInfo : InteractionInfo
    {
        private readonly List<HeldInteraction<TInteractionInfo>> _commands;
        private readonly object _commandsLock = new object();

        public ThreadSafeCommandHolder()
        {
            _commands = new List<HeldInteraction<TInteractionInfo>>();
        }

        public IEnumerable<HeldInteraction<TInteractionInfo>> Interactions
        {
            get
            {
                lock (_commandsLock)
                {
                    return new List<HeldInteraction<TInteractionInfo>>(_commands);
                }
            }
        }

        public HeldInteraction<TInteractionInfo>? TryMatch(IEnumerable<IInteractionMatcher> matchers,
            IDiscordInteraction interaction)
        {
            List<HeldInteraction<TInteractionInfo>> heldInteractions = Interactions.ToList();

            foreach (IInteractionMatcher matcher in matchers)
            {
                foreach (HeldInteraction<TInteractionInfo> heldInteraction in heldInteractions)
                {
                    if (matcher.Matches(interaction, heldInteraction.Info))
                    {
                        return heldInteraction;
                    }
                }
            }

            return null;
        }

        public TInteractionInfo AddCommand(TInteractionInfo info, ICommandExecutor<TInteractionInfo> executor)
        {
            lock (_commandsLock)
            {
                _commands.Add(new HeldInteraction<TInteractionInfo>(info, executor));
            }

            return info;
        }

        public void RemoveCommands()
        {
            lock (_commandsLock)
            {
                _commands.Clear();
            }
        }
    }
}