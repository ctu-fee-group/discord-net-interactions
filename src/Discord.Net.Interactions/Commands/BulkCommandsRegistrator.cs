using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.Rest;

namespace Discord.Net.Interactions.Commands
{
    /// <summary>
    /// Registers all commands at once
    /// </summary>
    public class BulkCommandsRegistrator<TInteractionInfo> : ICommandsRegistrator<TInteractionInfo>
        where TInteractionInfo : SlashCommandInfo
    {
        private readonly DiscordRestClient _client;
        private readonly ICommandPermissionsResolver<TInteractionInfo> _commandPermissionsResolver;

        public BulkCommandsRegistrator(DiscordRestClient client,
            ICommandPermissionsResolver<TInteractionInfo> commandPermissionsResolver)
        {
            _client = client;
            _commandPermissionsResolver = commandPermissionsResolver;
        }

        public Task RegisterCommandsAsync(ICommandHolder<TInteractionInfo> holder, CancellationToken token = default)
        {
            List<TInteractionInfo> globalInfos = new List<TInteractionInfo>();
            Dictionary<ulong, List<TInteractionInfo>> guildInfos = new Dictionary<ulong, List<TInteractionInfo>>();

            foreach (TInteractionInfo info in holder.Interactions.Select(x => x.Info))
            {
                if (info.Global)
                {
                    globalInfos.Add(info);
                }
                else
                {
                    if (info.GuildId is null)
                    {
                        throw new InvalidOperationException("Guild id cannot be null when the command is not global");
                    }

                    if (!guildInfos.TryGetValue((ulong)info.GuildId, out List<TInteractionInfo>? specificGuildInfos))
                    {
                        specificGuildInfos = new List<TInteractionInfo>();
                        guildInfos.Add((ulong)info.GuildId, specificGuildInfos);
                    }

                    specificGuildInfos.Add(info);
                }
            }

            List<Task> tasks = new List<Task>();
            tasks.Add(RegisterGlobalCommandsAsync(globalInfos, token));
            foreach (KeyValuePair<ulong, List<TInteractionInfo>> guildCommandData in guildInfos)
            {
                tasks.Add(RegisterGuildCommandsAsync(guildCommandData.Key, guildCommandData.Value, token));
            }

            return Task.WhenAll(tasks);
        }

        private async Task RegisterGlobalCommandsAsync(List<TInteractionInfo> globalInfos,
            CancellationToken cancellationToken)
        {
            IReadOnlyCollection<RestGlobalCommand>? registeredCommands = await _client.BulkOverwriteGlobalCommands(
                globalInfos.Select(x => x.BuiltCommand).ToArray(),
                new RequestOptions() { CancelToken = cancellationToken });

            foreach (RestGlobalCommand registeredCommand in registeredCommands)
            {
                TInteractionInfo matchedCommand =
                    globalInfos.First(x => x.BuiltCommand.Name == registeredCommand.Name);

                matchedCommand.Command = registeredCommand;
                matchedCommand.Registered = true;
            }

            // Persmissions of global commands are not supported currently
        }

        private async Task RegisterGuildCommandsAsync(ulong guildId, List<TInteractionInfo> guildInfos,
            CancellationToken cancellationToken)
        {
            IReadOnlyCollection<RestGuildCommand>? registeredCommands = await _client.BulkOverwriteGuildCommands(
                guildInfos.Select(x => x.BuiltCommand).ToArray(),
                guildId,
                new RequestOptions() { CancelToken = cancellationToken });

            Dictionary<ulong, ApplicationCommandPermission[]> permissions = new();

            foreach (RestGuildCommand registeredCommand in registeredCommands)
            {
                TInteractionInfo matchedCommand =
                    guildInfos.First(x => x.BuiltCommand.Name == registeredCommand.Name);

                matchedCommand.Command = registeredCommand;
                matchedCommand.Registered = true;

                // TODO: make bulk operation for this?
                ApplicationCommandPermission[] commandPermissions =
                    (await _commandPermissionsResolver.GetCommandPermissionsAsync(matchedCommand, cancellationToken))
                    .ToArray();

                if (commandPermissions.Length > 0)
                {
                    permissions.Add(registeredCommand.Id,
                        commandPermissions
                    );
                }
            }

            if (permissions.Count > 0)
            {
                await _client.BatchEditGuildCommandPermissions(guildId, permissions);
            }
        }

        public Task UnregisterCommandsAsync(ICommandHolder<TInteractionInfo> holder, CancellationToken token = default)
        {
            List<Task> tasks = new List<Task>();
            tasks.Add(_client.BulkOverwriteGlobalCommands(Array.Empty<SlashCommandCreationProperties>(),
                new RequestOptions() { CancelToken = token }));

            foreach (ulong guildId in GetGuilds(holder))
            {
                tasks.Add(_client.BulkOverwriteGuildCommands(Array.Empty<SlashCommandCreationProperties>(), guildId,
                    new RequestOptions() { CancelToken = token }));
            }

            return Task.WhenAll(tasks);
        }

        public Task RefreshCommandsAndPermissionsAsync(ICommandHolder<TInteractionInfo> holder,
            CancellationToken token = default)
        {
            return RegisterCommandsAsync(holder, token);
        }

        private IEnumerable<ulong> GetGuilds(ICommandHolder<TInteractionInfo> holder)
        {
            return holder.Interactions
                .Select(x => x.Info.GuildId)
                .Where(x => x is not null)
                .Cast<ulong>()
                .Distinct();
        }
    }
}