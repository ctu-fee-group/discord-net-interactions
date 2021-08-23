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
    /// Command registrator registering commands one by one calling discord API, making it slow, because of rate limiting
    /// </summary>
    public class OneByOneCommandsRegistrator<TInteractionInfo> : ICommandsRegistrator, IDisposable
        where TInteractionInfo : SlashCommandInfo
    {
        private readonly DiscordRestClient _client;
        private readonly CommandCache _cache;
        private readonly ICommandPermissionsResolver<TInteractionInfo> _commandPermissionsResolver;
        private readonly IGuildResolver<TInteractionInfo> _guildResolver;

        public OneByOneCommandsRegistrator(
            ICommandPermissionsResolver<TInteractionInfo> commandPermissionsResolver,
            IGuildResolver<TInteractionInfo> guildResolver,
            DiscordRestClient client)
        {
            _commandPermissionsResolver = commandPermissionsResolver;
            _guildResolver = guildResolver;
            _client = client;
            _cache = new CommandCache(client);
        }

        public async Task RegisterCommandsAsync(IInteractionHolder holder, CancellationToken token = default)
        {
            _cache.ResetIfNeeded();
            var registrationTasks = new List<Task>();
            var guildCommands = new List<TInteractionInfo>();

            foreach (var commandInfo in holder.Interactions.Select(x => x.Info).OfType<TInteractionInfo>())
            {
                commandInfo.BuiltCommand.DefaultPermission =
                    await _commandPermissionsResolver.IsForEveryoneAsync(commandInfo, token);

                if (commandInfo.Global)
                {
                    registrationTasks.Add(RegisterGlobalCommandAsync(commandInfo, token));
                }
                else
                {
                    guildCommands.Add(commandInfo);
                }
            }

            var guildCommandsData = await _guildResolver.GetGuildsBulkAsync(guildCommands);
            foreach (var guildCommandData in guildCommandsData)
            {
                registrationTasks.Add(RegisterGuildCommandsAsync(guildCommandData.Key, guildCommandData.Value, token));
            }

            await Task.WhenAll(registrationTasks);
        }

        public async Task RegisterGuildCommandsAsync(ulong guildId, IInteractionHolder holder,
            CancellationToken token = default)
        {
            _cache.ResetIfNeeded();
            var guildCommands = await _guildResolver.FilterGuildCommandsAsync(guildId,
                holder.Interactions.Select(x => x.Info).OfType<TInteractionInfo>().ToList());

            await RegisterGuildCommandsAsync(guildId, guildCommands, token);
        }

        public async Task UnregisterCommandsAsync(IInteractionHolder holder, CancellationToken token = default)
        {
            // TODO: merge Register and Unregister somehow (they are the same except called register/unregister functions)
            _cache.ResetIfNeeded();
            var unregistrationTasks = new List<Task>();
            var guildCommands = new List<TInteractionInfo>();

            foreach (var commandInfo in holder.Interactions.Select(x => x.Info).OfType<TInteractionInfo>())
            {
                if (commandInfo.Global)
                {
                    unregistrationTasks.Add(UnregisterGlobalCommandAsync(commandInfo, token));
                }
                else
                {
                    guildCommands.Add(commandInfo);
                }
            }

            var guildCommandsData = await _guildResolver.GetGuildsBulkAsync(guildCommands);
            foreach (var guildCommandData in guildCommandsData)
            {
                unregistrationTasks.Add(UnregisterGuildCommandsAsync(guildCommandData.Key, guildCommandData.Value,
                    token));
            }

            await Task.WhenAll(unregistrationTasks);
        }

        public async Task UnregisterGuildCommandsAsync(ulong guildId, IInteractionHolder holder,
            CancellationToken token = default)
        {
            _cache.ResetIfNeeded();
            var guildCommands = await _guildResolver.FilterGuildCommandsAsync(guildId,
                holder.Interactions.Select(x => x.Info).OfType<TInteractionInfo>().ToList());

            await UnregisterGuildCommandsAsync(guildId, guildCommands, token);
        }

        public async Task RefreshGuildCommandsAndPermissionsAsync(ulong guildId, IInteractionHolder holder,
            CancellationToken token = default)
        {
            _cache.ResetIfNeeded();
            var guildCommands = await _guildResolver.FilterGuildCommandsAsync(guildId,
                holder.Interactions.Select(x => x.Info).OfType<TInteractionInfo>().ToList());

            await RefreshGuildCommandsAsync(guildId, guildCommands, token);
        }

        public async Task RefreshCommandsAndPermissionsAsync(IInteractionHolder holder,
            CancellationToken token = default)
        {
            // TODO: merge Register and Unregister somehow (they are the same except called register/unregister functions)
            _cache.ResetIfNeeded();
            var refreshTasks = new List<Task>();
            var guildCommands = new List<TInteractionInfo>();

            foreach (var commandInfo in holder.Interactions.Select(x => x.Info).OfType<TInteractionInfo>())
            {
                commandInfo.BuiltCommand.DefaultPermission =
                    await _commandPermissionsResolver.IsForEveryoneAsync(commandInfo, token);

                if (commandInfo.Global)
                {
                    refreshTasks.Add(RefreshGlobalCommandAsync(commandInfo, token));
                }
                else
                {
                    guildCommands.Add(commandInfo);
                }
            }

            var guildCommandsData = await _guildResolver.GetGuildsBulkAsync(guildCommands);
            foreach (var guildCommandData in guildCommandsData)
            {
                refreshTasks.Add(RefreshGuildCommandsAsync(guildCommandData.Key, guildCommandData.Value, token));
            }

            await Task.WhenAll(refreshTasks);
        }

        public void Dispose()
        {
            _cache.Dispose();
        }

        private Task RegisterGuildCommandsAsync(ulong guildId, IEnumerable<TInteractionInfo> commands,
            CancellationToken token)
        {
            return Task.WhenAll(commands.Select(x => RegisterGuildCommandAsync(guildId, x, token)));
        }

        private Task UnregisterGuildCommandsAsync(ulong guildId, IEnumerable<TInteractionInfo> commands,
            CancellationToken token)
        {
            return Task.WhenAll(commands.Select(x => UnregisterGuildCommandAsync(guildId, x, token)));
        }

        private Task RefreshGuildCommandsAsync(ulong guildId, IEnumerable<TInteractionInfo> commands,
            CancellationToken token)
        {
            return Task.WhenAll(commands.Select(x => RefreshGuildCommandAsync(guildId, x, token)));
        }

        private async Task RegisterGlobalCommandAsync(TInteractionInfo commandInfo, CancellationToken token)
        {
            if (await RefreshGlobalCommandAsync(commandInfo, token))
            {
                return;
            }

            await _client.CreateGlobalCommand(commandInfo.BuiltCommand,
                new RequestOptions() { CancelToken = token });
        }

        private async Task RegisterGuildCommandAsync(ulong guildId, TInteractionInfo commandInfo,
            CancellationToken token)
        {
            if (await RefreshGuildCommandAsync(guildId, commandInfo, token))
            {
                return;
            }
          
            var command = await _client.CreateGuildCommand(commandInfo.BuiltCommand, guildId,
                new RequestOptions() { CancelToken = token });
            await RefreshCommandPermissionsAsync(commandInfo, command, token);
        }

        private async Task UnregisterGlobalCommandAsync(TInteractionInfo commandInfo, CancellationToken token)
        {
            var command = await _cache.GetGlobalCommand(commandInfo.BuiltCommand.Name, token);
            if (command is not null)
            {
                await command.DeleteAsync();
            }
        }

        private async Task UnregisterGuildCommandAsync(ulong guildId, TInteractionInfo commandInfo,
            CancellationToken token)
        {
            var command = await _cache.GetGuildCommand(guildId, commandInfo.BuiltCommand.Name, token);
            if (command is not null)
            {
                await command.DeleteAsync();
            }
        }

        private async Task<bool> RefreshGlobalCommandAsync(TInteractionInfo commandInfo, CancellationToken token)
        {
            var command = await _cache.GetGlobalCommand(commandInfo.BuiltCommand.Name, token);
            if (command is null)
            {
                return false;
            }

            if (!command.MatchesCreationProperties(commandInfo.BuiltCommand))
            {
                await command.ModifyAsync(props =>
                {
                    props.Description = commandInfo.BuiltCommand.Description;
                    //props.Name = commandInfo.BuiltCommand.Name;
                    props.DefaultPermission = commandInfo.BuiltCommand.DefaultPermission;
                    props.Options = commandInfo.BuiltCommand.Options;
                }, new RequestOptions() { CancelToken = token });
            }

            return true;
        }

        private async Task<bool> RefreshGuildCommandAsync(ulong guildId, TInteractionInfo commandInfo,
            CancellationToken token)
        {
            var command = await _cache.GetGuildCommand(guildId, commandInfo.BuiltCommand.Name, token);
            if (command is null)
            {
                return false;
            }

            if (!command.MatchesCreationProperties(commandInfo.BuiltCommand))
            {
                await command.ModifyAsync(props =>
                {
                    props.Description = commandInfo.BuiltCommand.Description;
                    //props.Name = commandInfo.BuiltCommand.Name;
                    props.DefaultPermission = commandInfo.BuiltCommand.DefaultPermission;
                    props.Options = commandInfo.BuiltCommand.Options;
                }, new RequestOptions() { CancelToken = token });
            }

            await RefreshCommandPermissionsAsync(commandInfo, command, token);
            return true;
        }

        private async Task RefreshCommandPermissionsAsync(TInteractionInfo info, RestGuildCommand command,
            CancellationToken token)
        {
            var currentPermissions = await command.GetCommandPermission(new RequestOptions() { CancelToken = token });
            var correctPermissions =
                (await _commandPermissionsResolver.GetCommandPermissionsAsync(info, token)).ToArray();

            // TODO: remove correctPermissions.Length > 0 when it is fixed
            if (correctPermissions.Length > 0 && (currentPermissions is null || !currentPermissions.MatchesPermissions(correctPermissions)))
            {
                await command.ModifyCommandPermissions(correctPermissions,
                    new RequestOptions() { CancelToken = token });
            }
        }
    }
}