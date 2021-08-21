using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Interactions.Abstractions
{
    /// <summary>
    /// Resolves permissions for slash commands
    /// </summary>
    /// <typeparam name="TInteractionInfo"></typeparam>
    public interface ICommandPermissionsResolver<in TInteractionInfo>
        where TInteractionInfo : InteractionInfo
    {
        /// <summary>
        /// Whether everyone should be able to execute specified command
        /// </summary>
        /// <remarks>
        /// If this returns true, default permission will be set to true
        /// </remarks>
        /// <param name="info">Command that should have permissions checked</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> IsForEveryoneAsync(TInteractionInfo info, CancellationToken cancellationToken);

        /// <summary>
        /// If specified user has permission to execute specified command
        /// </summary>
        /// <param name="user">User that should be checked for permissions</param>
        /// <param name="info">Command that is being checked</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> HasPermissionAsync(IUser user, TInteractionInfo info, CancellationToken cancellationToken);

        /// <summary>
        /// What permissions should the command be assigned
        /// </summary>
        /// <remarks>
        /// 10 permissions can be returned at most. (discord limitation)
        /// </remarks>
        /// <param name="info"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Permissions of the command</returns>
        public Task<IEnumerable<ApplicationCommandPermission>> GetCommandPermissionsAsync(TInteractionInfo info, CancellationToken cancellationToken);
    }
}