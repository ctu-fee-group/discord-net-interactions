using System;
using System.Collections;
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
    public class BulkCommandsRegistrator<TInteractionInfo> : ICommandsRegistrator
        where TInteractionInfo : SlashCommandInfo
    {
        private readonly DiscordRestClient _client;
        private readonly ICommandPermissionsResolver<TInteractionInfo> _commandPermissionsResolver;
        private readonly IGuildResolver<TInteractionInfo> _guildResolver;

        public BulkCommandsRegistrator(DiscordRestClient client,
            ICommandPermissionsResolver<TInteractionInfo> commandPermissionsResolver,
            IGuildResolver<TInteractionInfo> guildResolver)
        {
            _guildResolver = guildResolver;
            _client = client;
            _commandPermissionsResolver = commandPermissionsResolver;
        }

        public async Task RegisterCommandsAsync(IInteractionHolder holder, CancellationToken token = default)
        {
            var globalInfos = new List<TInteractionInfo>();
            var guildInfos = new List<TInteractionInfo>();

            foreach (var info in holder.Interactions.Select(x => x.Info).OfType<TInteractionInfo>())
            {
                info.BuiltCommand.DefaultPermission = await _commandPermissionsResolver.IsForEveryoneAsync(info, token);
                if (info.Global)
                {
                    globalInfos.Add(info);
                }
                else
                {
                    guildInfos.Add(info);
                }
            }

            var tasks = new List<Task>();
            tasks.Add(RegisterGlobalCommandsAsync(globalInfos, token));
            tasks.Add(RegisterGuildCommandsAsync(guildInfos, token));


            await Task.WhenAll(tasks);
        }

        private async Task RegisterGuildCommandsAsync(List<TInteractionInfo> guildInfos,
            CancellationToken token = new CancellationToken())
        {
            var guildCommandsData = await _guildResolver.GetGuildsBulkAsync(guildInfos);
            var tasks = new List<Task>();
            foreach (var guildCommandData in guildCommandsData)
            {
                tasks.Add(RegisterGuildCommandsAsync(guildCommandData.Key, guildCommandData.Value, token));
            }

            await Task.WhenAll(tasks);
        }

        public async Task RegisterGuildCommandsAsync(ulong guildId, IInteractionHolder holder,
            CancellationToken token = default)
        {
            IEnumerable<TInteractionInfo> commands =
                holder.Interactions.Select(x => x.Info).OfType<TInteractionInfo>();
            commands = await _guildResolver.FilterGuildCommandsAsync(guildId, commands);

            await RegisterGuildCommandsAsync(guildId, commands.ToList(), token);
        }

        private Task RegisterGlobalCommandsAsync(List<TInteractionInfo> globalInfos,
            CancellationToken cancellationToken)
        {
            return _client.BulkOverwriteGlobalCommands(
                globalInfos.Select(x => x.BuiltCommand).ToArray(),
                new RequestOptions() { CancelToken = cancellationToken });
            // Persmissions of global commands are not supported currently
        }

        private async Task RegisterGuildCommandsAsync(ulong guildId, IEnumerable<TInteractionInfo> guildInfos,
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

        public async Task UnregisterCommandsAsync(IInteractionHolder holder,
            CancellationToken token = default)
        {
            List<Task> tasks = new List<Task>();
            tasks.Add(_client.BulkOverwriteGlobalCommands(Array.Empty<SlashCommandCreationProperties>(),
                new RequestOptions() { CancelToken = token }));

            foreach (var guildId in await GetGuilds(holder))
            {
                tasks.Add(_client.BulkOverwriteGuildCommands(Array.Empty<SlashCommandCreationProperties>(), guildId,
                    new RequestOptions() { CancelToken = token }));
            }

            await Task.WhenAll(tasks);
        }

        public Task UnregisterGuildCommandsAsync(ulong guildId, IInteractionHolder holder,
            CancellationToken token = default)
        {
            return _client.BulkOverwriteGuildCommands(Array.Empty<SlashCommandCreationProperties>(), guildId,
                new RequestOptions() { CancelToken = token });
        }

        public Task RefreshGuildCommandsAndPermissionsAsync(ulong guildId, IInteractionHolder holder,
            CancellationToken token = default)
        {
            return RegisterGuildCommandsAsync(guildId, holder, token);
        }

        public Task RefreshCommandsAndPermissionsAsync(IInteractionHolder holder,
            CancellationToken token = default)
        {
            return RegisterCommandsAsync(holder, token);
        }

        private async Task<IEnumerable<ulong>> GetGuilds(IInteractionHolder holder)
        {
            var commands = holder.Interactions
                .Select(x => x.Info)
                .OfType<TInteractionInfo>();

            return (await _guildResolver.GetGuildsBulkAsync(commands))
                .Select(x => x.Key);
        }
    }
}