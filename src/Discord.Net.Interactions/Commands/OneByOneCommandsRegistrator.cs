using System;
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
    public class OneByOneCommandsRegistrator<TSlashInfo> : ICommandsRegistrator<TSlashInfo>
        where TSlashInfo : SlashCommandInfo
    {
        private readonly DiscordRestClient _client;
        private readonly CommandCache _cache;
        private readonly ICommandPermissionsResolver<TSlashInfo> _commandPermissionsResolver;

        public OneByOneCommandsRegistrator(ICommandPermissionsResolver<TSlashInfo> commandPermissionsResolver,
            DiscordRestClient client)
        {
            _commandPermissionsResolver = commandPermissionsResolver;
            _client = client;
            _cache = new CommandCache(client);
        }

        public async Task RegisterCommandsAsync(ICommandHolder<TSlashInfo> holder, CancellationToken token = default)
        {
            foreach (HeldSlashCommand<TSlashInfo> heldCommand in holder.Commands)
            {
                try
                {
                    await RegisterCommandAsync(heldCommand.Info, token);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(
                        $@"Could not register a command {heldCommand.Info.BuiltCommand.Name}", e);
                }
            }
        }

        public Task UnregisterCommandsAsync(ICommandHolder<TSlashInfo> holder, CancellationToken token = default)
        {
            return Task.WhenAll(
                holder.Commands
                    .Select(x => UnregisterCommandAsync(x.Info, token)));
        }

        public Task RefreshCommandsAndPermissionsAsync(ICommandHolder<TSlashInfo> holder,
            CancellationToken token = default)
        {
            return Task.WhenAll(
                holder.Commands.Select(x => RefreshCommandAsync(x.Info, token)));
        }

        private async Task RefreshCommandAsync(TSlashInfo info, CancellationToken token = new CancellationToken())
        {
            if (info.Command == null)
            {
                throw new InvalidOperationException("Cannot refresh without the command registered");
            }

            await ModifyCommand(info, token);
            await RefreshPermissions(info, token);
        }

        private async Task<IApplicationCommand> RegisterCommandAsync(TSlashInfo info,
            CancellationToken token = new CancellationToken())
        {
            if (info.Command == null)
            {
                SlashCommandCreationProperties command = await SetDefaultPermissionAsync(info, token);

                if (info.Global)
                {
                    info.Command = await CreateGlobalCommand(info, token);
                }
                else
                {
                    info.Command = await CreateGuildCommand(info, token);
                }
            }

            info.Registered = true;
            return info.Command;
        }

        private async Task UnregisterCommandAsync(TSlashInfo info,
            CancellationToken token = new CancellationToken())
        {
            if (info.Command != null)
            {
                await info.Command.DeleteAsync(new()
                {
                    CancelToken = token
                });
                info.Command = null;
            }

            info.Registered = false;
        }

        private async Task ModifyCommand(TSlashInfo info, CancellationToken token = new CancellationToken())
        {
            if (info.Command is RestGlobalCommand globalCommand &&
                !info.Command.MatchesCreationProperties(info.BuiltCommand))
            {
                await globalCommand.ModifyAsync(props =>
                    ModifyCommandProperties(info, props, token).GetAwaiter().GetResult());
            }
            else if (info.Command is RestGuildCommand guildCommand)
            {
                if (!info.Command.MatchesCreationProperties(info.BuiltCommand))
                {
                    await guildCommand.ModifyAsync(props =>
                        ModifyCommandProperties(info, props, token).GetAwaiter().GetResult());
                }

                await RefreshPermissions(info, token);
            }
        }

        private async Task RefreshPermissions(TSlashInfo info, CancellationToken token = new CancellationToken())
        {
            if (info.Command is RestGlobalCommand)
            {
                return; // Global commands cannot have permissions (at least not in Discord.NET yet)
            }
            else if (info.Command is RestGuildCommand guildCommand)
            {
                ApplicationCommandPermission[] permissions =
                    (await _commandPermissionsResolver.GetCommandPermissionsAsync(info, token)).ToArray();
                GuildApplicationCommandPermission? commandPermission = await guildCommand.GetCommandPermission();

                if (permissions.Length > 0 &&
                    (commandPermission == null || !commandPermission.MatchesPermissions(permissions)))
                {
                    await guildCommand.ModifyCommandPermissions(permissions);
                }
            }
        }

        private async Task<SlashCommandCreationProperties> SetDefaultPermissionAsync(
            TSlashInfo info,
            CancellationToken token = new CancellationToken())
        {
            info.BuiltCommand.DefaultPermission =
                await _commandPermissionsResolver.IsForEveryoneAsync(info, token);
            return info.BuiltCommand;
        }

        private async Task<ApplicationCommandProperties> ModifyCommandProperties(
            TSlashInfo info,
            ApplicationCommandProperties command,
            CancellationToken token = new CancellationToken())
        {
            command.Description = info.BuiltCommand.Description;
            command.Name = info.BuiltCommand.Name;
            command.Options = info.BuiltCommand.Options;
            command.DefaultPermission =
                await _commandPermissionsResolver.IsForEveryoneAsync(info, token);
            return command;
        }

        private async Task<RestApplicationCommand> CreateGlobalCommand(TSlashInfo info,
            CancellationToken token = new CancellationToken())
        {
            RestGlobalCommand? globalCommand = await _cache.GetGlobalCommand(info.BuiltCommand.Name, token);

            if (globalCommand != null)
            {
                info.Command = globalCommand;
                await ModifyCommand(info, token);
            }
            else
            {
                globalCommand = await _client.CreateGlobalCommand(info.BuiltCommand, new RequestOptions()
                {
                    CancelToken = token
                });
                info.Command = globalCommand;

                await RefreshPermissions(info, token);
            }

            return globalCommand;
        }

        private async Task<RestApplicationCommand> CreateGuildCommand(TSlashInfo info,
            CancellationToken token = new CancellationToken())
        {
            if (info.GuildId == null)
            {
                throw new ArgumentException("GuildId cannot be null for guild commands");
            }

            RestGuildCommand? guildCommand =
                await _cache.GetGuildCommand((ulong)info.GuildId, info.BuiltCommand.Name, token);

            if (guildCommand != null)
            {
                info.Command = guildCommand;
                await ModifyCommand(info, token);
            }
            else
            {
                guildCommand = await _client.CreateGuildCommand(info.BuiltCommand, (ulong)info.GuildId,
                    new RequestOptions()
                    {
                        CancelToken = token
                    });
                info.Command = guildCommand;

                await RefreshPermissions(info, token);
            }

            return guildCommand;
        }
    }
}