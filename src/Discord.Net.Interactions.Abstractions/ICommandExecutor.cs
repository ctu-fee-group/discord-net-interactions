using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Discord.Net.Interactions.Abstractions
{
    /// <summary>
    /// Executor of a slash command interaction
    /// </summary>
    public interface ICommandExecutor<in TSlashInfo>
        where TSlashInfo : SlashCommandInfo
    {
        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="info">Information about the slash command</param>
        /// <param name="command">Slash command interaction</param>
        /// <param name="token">Cancel token</param>
        /// <returns></returns>
        public Task TryExecuteCommand(TSlashInfo info, SocketSlashCommand command, CancellationToken token = default);
    }
}