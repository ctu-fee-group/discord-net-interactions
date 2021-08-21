using System.Collections.Generic;
using System.Linq;
using Discord.Net.Interactions.Abstractions;

namespace Discord.Net.Interactions.Commands
{
    /// <summary>
    /// Thread-safe implementation of CommandHolder using locks to achieve thread safety
    /// </summary>
    public class ThreadSafeInteractionHolder : IInteractionHolder
    {
        private readonly List<HeldInteraction> _commands;
        private readonly object _commandsLock = new object();

        public ThreadSafeInteractionHolder()
        {
            _commands = new List<HeldInteraction>();
        }

        public IEnumerable<HeldInteraction> Interactions
        {
            get
            {
                lock (_commandsLock)
                {
                    return new List<HeldInteraction>(_commands);
                }
            }
        }

        public HeldInteraction? TryMatch(IEnumerable<IInteractionMatcher> matchers,
            IDiscordInteraction interaction)
        {
            List<HeldInteraction> heldInteractions = Interactions.ToList();

            foreach (IInteractionMatcher matcher in matchers)
            {
                foreach (HeldInteraction heldInteraction in heldInteractions)
                {
                    if (matcher.Matches(interaction, heldInteraction.Info))
                    {
                        return heldInteraction;
                    }
                }
            }

            return null;
        }

        public void AddInteraction(InteractionInfo info, IInteractionExecutor executor)
        {
            lock (_commandsLock)
            {
                _commands.Add(new HeldInteraction(info, executor));
            }
        }

        public void RemoveInteraction(InteractionInfo info)
        {
            lock (_commandsLock)
            {
                _commands.RemoveAll(x => x.Info == info);
            }
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