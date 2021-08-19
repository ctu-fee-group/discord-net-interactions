using System.Collections.Generic;

namespace Discord.NET.InteractionsService.Abstractions
{
    /// <summary>
    /// Holds SlashCommand information, so it is known what executor to execute
    /// </summary>
    /// <param name="Info"></param>
    /// <param name="Executor"></param>
    public record HeldSlashCommand<TSlashInfo>(TSlashInfo Info, ICommandExecutor Executor)
        where TSlashInfo : SlashCommandInfo;
    
    /// <summary>
    /// Interface supporting registration and holding of slash commands
    /// </summary>
    public interface ICommandHolder<TSlashInfo>
        where TSlashInfo : SlashCommandInfo
    {
        public IEnumerable<HeldSlashCommand<TSlashInfo>> Commands { get; }

        /// <summary>
        /// Tries to get a slash command in list of commands by its name
        /// If command is not found, null is returned
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Slash command if it was found, otherwise null</returns>
        public HeldSlashCommand<TSlashInfo>? TryGetSlashCommand(string name);
        
        /// <summary>
        /// Save command to collection
        /// </summary>
        /// <param name="info"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        public SlashCommandInfo AddCommand(TSlashInfo info, ICommandExecutor executor);
        
        /// <summary>
        /// Remove all commands from collection
        /// </summary>
        /// <param name="token"></param>
        public void RemoveCommands();
    }
}