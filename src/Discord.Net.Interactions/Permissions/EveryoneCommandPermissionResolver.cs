using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public Task<bool> IsForEveryoneAsync(TSlashInfo info, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<bool> HasPermissionAsync(IUser user, TSlashInfo info, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<IEnumerable<ApplicationCommandPermission>> GetCommandPermissionsAsync(TSlashInfo info, CancellationToken cancellationToken)
        {
            return Task.FromResult(Enumerable.Empty<ApplicationCommandPermission>()); 
        }
    }
}