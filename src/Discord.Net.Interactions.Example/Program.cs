using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.Net.Interactions.DI;
using Discord.Net.Interactions.Example.Commands;
using Discord.Net.Interactions.Executors;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Discord.Net.Interactions.Example
{
    public delegate void StopBot();

    class Program
    {
        private static CancellationTokenSource _runToken;

        static async Task Main(string[] args)
        {
            IServiceProvider services = CreateServices();
            await RunBot(
                services.GetRequiredService<DiscordSocketClient>(),
                services.GetRequiredService<IOptions<BotOptions>>().Value,
                services.GetRequiredService<InteractionsService>(),
                services.GetRequiredService<ILogger<Program>>());
        }

        static IServiceProvider CreateServices()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .Build();

            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<DiscordRestClient>(p => p.GetRequiredService<DiscordSocketClient>().Rest)
                .AddDefaultInteractionService()
                .AddOneByOneCommandRegistrator<SlashCommandInfo>() // commands will be registered one by one
                .AddEveryoneCommandPermissionResolver<SlashCommandInfo>() // everyone will have permission to use the command
                .AddOneGuildResolver<SlashCommandInfo>(configuration.GetSection("Commands")) // the commands will be registered only in one guild specified by configuration
                .AddCommandGroup<ControlCommandGroup>() // Group PingCommandGroup will be registered and used
                .AddCommandGroup<PingCommandGroup>()
                // logging
                .AddLogging(builder => builder
                    .AddConsole())
                // configs
                .Configure<CommandsOptions>(configuration.GetSection("Commands"))
                .Configure<DiscordSocketConfig>(configuration.GetSection("Bot"))
                .Configure<DiscordSocketConfig>(p => p.AlwaysAcknowledgeInteractions = false)
                .Configure<BotOptions>(configuration.GetSection("Bot"))
                .AddSingleton<DiscordSocketConfig>(p => p.GetRequiredService<IOptions<DiscordSocketConfig>>().Value)
                // option to stop the bot from any service
                .AddSingleton<StopBot>(StopBot)
                .BuildServiceProvider();
        }

        static async Task RunBot(DiscordSocketClient client, BotOptions options,
            InteractionsService interactionsService, ILogger logger)
        {
            await client.LoginAsync(TokenType.Bot, options.Token);
            await client.StartAsync();
            
            client.Log += message =>
            {
                logger.LogInformation(message.Exception, message.Message);
                return Task.CompletedTask;
            };

            client.Ready += async () =>
            {
                // register commands and interaction created handler
                await interactionsService.StartAsync();
            };

            _runToken = new CancellationTokenSource();
            try
            {
                await Task.Delay(-1, _runToken.Token);
            }
            catch (OperationCanceledException e)
            {
                
            }

            // unregister commands and interaction created handler
            await interactionsService.StopAsync();
            await client.StopAsync();
            await client.LogoutAsync();
        }

        static void StopBot()
        {
            _runToken?.Cancel();
        }
    }
}