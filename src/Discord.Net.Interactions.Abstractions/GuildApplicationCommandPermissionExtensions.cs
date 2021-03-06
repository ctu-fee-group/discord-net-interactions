using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Net.Interactions.Abstractions
{
    public static class GuildApplicationCommandPermissionExtensions
    {
        /// <summary>
        /// Check if given GuildApplicationCommandPermission matches ApplicationCommandPermission[]
        /// </summary>
        /// <param name="commandPermission"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public static bool MatchesPermissions(this GuildApplicationCommandPermission commandPermission,
            ApplicationCommandPermission[] permissions)
        {
            return Enumerable.SequenceEqual<ApplicationCommandPermission>(
                commandPermission.Permissions.OrderBy(x => x.TargetId),
                permissions.OrderBy(x => x.TargetId),
                new ApplicationCommandPermissionComparer());
        }

        private class ApplicationCommandPermissionComparer : IEqualityComparer<ApplicationCommandPermission>
        {
            public bool Equals(ApplicationCommandPermission? x, ApplicationCommandPermission? y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.TargetId == y.TargetId && x.TargetType == y.TargetType && x.Permission == y.Permission;
            }

            public int GetHashCode(ApplicationCommandPermission obj)
            {
                return HashCode.Combine(obj.TargetId, (int) obj.TargetType, obj.Permission);
            }
        }
    }
}