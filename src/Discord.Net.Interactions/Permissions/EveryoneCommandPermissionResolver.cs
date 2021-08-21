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
    /// <typeparam name="TInteractionInfo"></typeparam>
    public class EveryoneCommandPermissionResolver<TInteractionInfo>
        : ICommandPermissionsResolver<TInteractionInfo>
        where TInteractionInfo : InteractionInfo
    {
        public Task<bool> IsForEveryoneAsync(TInteractionInfo info, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<bool> HasPermissionAsync(IUser user, TInteractionInfo info, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<IEnumerable<ApplicationCommandPermission>> GetCommandPermissionsAsync(TInteractionInfo info, CancellationToken cancellationToken)
        {
            return Task.FromResult(Enumerable.Empty<ApplicationCommandPermission>()); 
        }
    }
}