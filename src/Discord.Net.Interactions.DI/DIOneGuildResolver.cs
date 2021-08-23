using Discord.Net.Interactions.Abstractions;
using Discord.Net.Interactions.Commands;
using Microsoft.Extensions.Options;

namespace Discord.Net.Interactions.DI
{
    public class DIOneGuildResolver<TSlashInfo> : OneGuildResolver<TSlashInfo>
        where TSlashInfo : SlashCommandInfo
    {
        public DIOneGuildResolver(IOptions<GuildOptions> options)
            : base(options.Value.GuildId)
        {
        }
    }
}