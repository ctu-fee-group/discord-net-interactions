using System.Collections.Generic;
using System.Linq;
using Discord.NET.InteractionsService.Abstractions;
using Discord.NET.InteractionsService.CommandsInfo;
using Discord.Rest;

namespace Discord.NET.InteractionsService.Commands
{
    /// <summary>
    /// Thread-safe implementation of CommandHolder
    /// </summary>
    public class CommandHolder : ICommandHolder
    {
        private readonly List<ICommandHolder.HeldSlashCommand> _commands;
        private readonly object _commandsLock = new object();
        
        public CommandHolder(DiscordRestClient client)
        {
            _commands = new List<ICommandHolder.HeldSlashCommand>();
        }

        public IEnumerable<ICommandHolder.HeldSlashCommand> Commands
        {
            get
            {
                lock (_commandsLock)
                {
                    return new List<ICommandHolder.HeldSlashCommand>(_commands);
                }
            }
        }

        public ICommandHolder.HeldSlashCommand? TryGetSlashCommand(string name)
        {
            lock (_commandsLock)
            {
                return _commands.FirstOrDefault(x => x.Info.BuiltCommand.Name == name);
            }
        }

        public SlashCommandInfo AddCommand(SlashCommandInfo info, ICommandExecutor executor)
        {
            lock (_commandsLock)
            {
                _commands.Add(new ICommandHolder.HeldSlashCommand(info, executor));
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