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
    public class CommandHolder<TSlashInfo> : ICommandHolder<TSlashInfo>
        where TSlashInfo : SlashCommandInfo
    {
        private readonly List<HeldSlashCommand<TSlashInfo>> _commands;
        private readonly object _commandsLock = new object();
        
        public CommandHolder()
        {
            _commands = new List<HeldSlashCommand<TSlashInfo>>();
        }

        public IEnumerable<HeldSlashCommand<TSlashInfo>> Commands
        {
            get
            {
                lock (_commandsLock)
                {
                    return new List<HeldSlashCommand<TSlashInfo>>(_commands);
                }
            }
        }

        public HeldSlashCommand<TSlashInfo>? TryGetSlashCommand(string name)
        {
            lock (_commandsLock)
            {
                return _commands.FirstOrDefault(x => x.Info.BuiltCommand.Name == name);
            }
        }

        public SlashCommandInfo AddCommand(TSlashInfo info, ICommandExecutor executor)
        {
            lock (_commandsLock)
            {
                _commands.Add(new HeldSlashCommand<TSlashInfo>(info, executor));
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