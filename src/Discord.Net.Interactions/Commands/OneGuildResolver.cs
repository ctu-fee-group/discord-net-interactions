using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;

namespace Discord.Net.Interactions.Commands
{
    public class OneGuildResolver<TSlashInfo> : IGuildResolver<TSlashInfo>
        where TSlashInfo : SlashCommandInfo
    {
        private ulong _guildId;
        private IEnumerable<ulong> _guildIdArray;

        public OneGuildResolver(ulong guildId)
        {
            GuildId = guildId;
        }

        public ulong GuildId
        {
            get => _guildId;
            set
            {
                _guildId = value;
                _guildIdArray = new[] { _guildId };
            }
        }

        public Task<IEnumerable<ulong>> GetGuildAsync(TSlashInfo info)
        {
            return Task.FromResult(_guildIdArray);
        }

        public Task<IReadOnlyDictionary<ulong, IEnumerable<TSlashInfo>>> GetGuildsBulkAsync(
            IEnumerable<TSlashInfo> infos)
        {
            var dictionary = new Dictionary<ulong, IEnumerable<TSlashInfo>>();
            dictionary.Add(_guildId, infos.Where(x => !x.Global));

            return Task.FromResult<IReadOnlyDictionary<ulong, IEnumerable<TSlashInfo>>>(dictionary);
        }

        public Task<IEnumerable<TSlashInfo>> FilterGuildCommandsAsync(ulong guildId, IEnumerable<TSlashInfo> infos)
        {
            if (guildId == _guildId)
            {
                return Task.FromResult(infos.Where(x => !x.Global));
            }

            return Task.FromResult(Enumerable.Empty<TSlashInfo>());
        }

        public Task<bool> IsGuildAssignedAsync(ulong guildId, TSlashInfo info)
        {
            return Task.FromResult(guildId == _guildId);
        }
    }
}