using System.Threading;
using System.Threading.Tasks;

namespace Discord.NET.InteractionsService.Abstractions
{
    /// <summary>
    /// Group of commands exposing a function for registering the commands
    /// </summary>
    public interface ICommandGroup<TSlashInfo>
        where TSlashInfo : SlashCommandInfo
    {
        /// <summary>
        /// Builds and registers slash commands
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task SetupCommandsAsync(ICommandHolder<TSlashInfo> holder, CancellationToken token = new CancellationToken());
    }
}