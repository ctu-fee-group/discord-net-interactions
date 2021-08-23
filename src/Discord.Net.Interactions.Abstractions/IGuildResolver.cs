using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.Net.Interactions.Abstractions
{
    public interface IGuildResolver<TSlashInfo>
        where TSlashInfo : SlashCommandInfo
    {
        /// <summary>
        /// Get what guilds should the specified command register to
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<IEnumerable<ulong>> GetGuildAsync(TSlashInfo info);
        
        /// <summary>
        /// Get what guilds should the specified commands be registered to
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<IReadOnlyDictionary<ulong, IEnumerable<TSlashInfo>>> GetGuildsBulkAsync(IEnumerable<TSlashInfo> infos);
        
        /// <summary>
        /// Get whether the specified commands are supposed to be assigned to the specified guild
        /// </summary>
        /// <param name="guildId"></param>
        /// <param name="infos"></param>
        /// <returns>What commands are assigned</returns>
        Task<IEnumerable<TSlashInfo>> FilterGuildCommandsAsync(ulong guildId, IEnumerable<TSlashInfo> infos);

        /// <summary>
        /// Get whether the specified command is supposed to be assigned to the specified guild
        /// </summary>
        /// <param name="guildId"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<bool> IsGuildAssignedAsync(ulong guildId, TSlashInfo info);
    }
}