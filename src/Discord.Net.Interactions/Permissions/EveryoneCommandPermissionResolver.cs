using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;

namespace Discord.Net.Interactions.Permissions
{
    /// <summary>
    /// Basic command permissions resolver that will simply allow everyone to use the command
    /// </summary>
    /// <typeparam name="TSlashInfo"></typeparam>
    public class EveryoneCommandPermissionResolver<TSlashInfo>
        : ICommandPermissionsResolver<TSlashInfo>
        where TSlashInfo : SlashCommandInfo
    {
        public Task<bool> IsForEveryone(TSlashInfo info)
        {
            return Task.FromResult(true);
        }

        public Task<bool> HasPermission(IUser user, TSlashInfo info)
        {
            return Task.FromResult(true);
        }

        public Task<IEnumerable<ApplicationCommandPermission>> GetCommandPermissions(TSlashInfo info)
        {
            return Task.FromResult(Enumerable.Empty<ApplicationCommandPermission>()); 
        }
    }
}