using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.Rest;

namespace Discord.Net.Interactions.Commands
{
    /// <summary>
    /// Cache commands in memory to avoid calling REST multiple times
    /// </summary>
    public class CommandCache : IDisposable
    {
        private const int CacheSeconds = 60 * 5;

        private readonly DiscordRestClient _client;
        private ulong? _applicationId;

        private IReadOnlyCollection<RestGlobalCommand>? _cachedGlobalCommands;
        private ConcurrentDictionary<ulong, IReadOnlyCollection<RestGuildCommand>>? _cachedGuildCommands;
        private DateTime _resetTime;

        private Task? _autoResetTask;
        private CancellationTokenSource? _source;
        private bool _autoResetting;
        private object _resetLock = new object();

        public CommandCache(DiscordRestClient client)
        {
            _resetTime = DateTime.Now;
            _client = client;
        }

        public void Reset(bool autoReset = true)
        {
            lock (_resetLock)
            {
                _resetTime = DateTime.Now;
                _cachedGlobalCommands = null;
                _cachedGuildCommands = null;

                if (_autoResetTask is not null)
                {
                    _source?.Cancel();
                    _source?.Dispose();
                    _source = null;
                    _autoResetTask = null;
                }

                _autoResetting = autoReset;

                if (autoReset)
                {
                    _source = new CancellationTokenSource();
                    _autoResetTask = Task.Run(async () =>
                    {
                        try
                        {
                            await Task.Delay(CacheSeconds, _source.Token);
                            Reset(false);
                        }
                        catch (OperationCanceledException)
                        {
                        }
                        catch (Exception e)
                        {
                        }
                    });
                }
            }
        }

        public void ResetIfNeeded(bool autoReset = true)
        {
            lock (_resetLock)
            {
                if (!_autoResetting)
                {
                    Reset(autoReset);
                }
            }
        }

        /// <summary>
        /// Obtrain guild commands either from cache or from Discord REST if needed
        /// </summary>
        /// <param name="guildId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<RestGuildCommand>> GetGuildCommands(ulong guildId,
            CancellationToken token = default)
        {
            var cachedGuildCommands = _cachedGuildCommands;

            if (cachedGuildCommands == null)
            {
                cachedGuildCommands = _cachedGuildCommands =
                    new ConcurrentDictionary<ulong, IReadOnlyCollection<RestGuildCommand>>();
            }

            if (!cachedGuildCommands.TryGetValue(guildId, out IReadOnlyCollection<RestGuildCommand>? guildCommands))
            {
                ulong applicationId = await GetApplicationId(token);
                guildCommands = (await _client.GetGuildApplicationCommands(guildId,
                        options: new RequestOptions() { CancelToken = token }))
                    .Where(x => x.ApplicationId == applicationId)
                    .ToList();
                cachedGuildCommands.TryAdd(guildId, guildCommands);
            }

            return guildCommands;
        }

        /// <summary>
        /// Obtain global commands either from cache or from Discord REST if needed
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<RestGlobalCommand>> GetGlobalCommands(CancellationToken token = default)
        {
            var cachedGlobalCommands = _cachedGlobalCommands;
            if (cachedGlobalCommands == null)
            {
                ulong applicationId = await GetApplicationId(token);
                cachedGlobalCommands = _cachedGlobalCommands =
                    (await _client.GetGlobalApplicationCommands(options: new RequestOptions() { CancelToken = token }))
                    .Where(x => x.ApplicationId == applicationId).ToList();
            }

            return cachedGlobalCommands;
        }

        public async Task<RestGuildCommand?> GetGuildCommand(ulong guildId, string name,
            CancellationToken token = default)
        {
            return (await GetGuildCommands(guildId, token))
                .FirstOrDefault(x => x.Name == name);
        }

        public async Task<RestGlobalCommand?> GetGlobalCommand(string name, CancellationToken token = default)
        {
            return (await GetGlobalCommands(token))
                .FirstOrDefault(x => x.Name == name);
        }

        private async Task<ulong> GetApplicationId(CancellationToken token = default)
        {
            _applicationId ??= (await _client.GetApplicationInfoAsync()).Id;
            return (ulong)_applicationId;
        }

        public void Dispose()
        {
            _autoResetTask?.Dispose();
            _source?.Cancel();
            _source?.Dispose();
        }
    }
}