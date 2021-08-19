using System;
using System.Net.Security;
using Discord.NET.InteractionsService.Abstractions;
using Discord.NET.InteractionsService.Commands;
using Discord.NET.InteractionsService.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Discord.NET.InteractionsService.DI
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds default InteractionHandler to the service collection
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="configure">Function that is to make code more readable, it is invoken with the same collection that was passed to the function. It should register CommandGroups</param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultInteractionHandler<TSlashInfo>(this IServiceCollection collection, Action<IServiceCollection>? configure = null)
            where TSlashInfo : SlashCommandInfo
        {
            collection
                .AddOptions<DICommandGroupsProvider<TSlashInfo>>();

            collection
                .AddSingleton<ICommandsGroupProvider<TSlashInfo>>(p =>
                {
                    DICommandGroupsProvider<TSlashInfo> provider = p.GetRequiredService<IOptions<DICommandGroupsProvider<TSlashInfo>>>().Value;
                    provider.Provider = p;

                    return provider;
                })
                .AddSingleton<ICommandHolder<TSlashInfo>, CommandHolder<TSlashInfo>>()
                .AddSingleton<InteractionHandler<TSlashInfo>>()
                .AddSingleton<ICommandsRegistrator<TSlashInfo>>(p => p.GetRequiredService<CommandsRegistrator<TSlashInfo>>())
                .AddSingleton<CommandsRegistrator<TSlashInfo>>();

            configure?.Invoke(collection);
            
            return collection;
        }
        
        /// <summary>
        /// Adds CommandGroup as singleton and configures DICommandGroupsProvider to provide this group command correctly
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IServiceCollection AddCommandGroup<TGroup, TSlashInfo>(this IServiceCollection collection)
            where TGroup : class, ICommandGroup<TSlashInfo>
            where TSlashInfo : SlashCommandInfo
        {
            collection.AddSingleton<TGroup>();

            collection.Configure<DICommandGroupsProvider<TSlashInfo>>(handler =>
                handler.RegisterGroupType(typeof(TGroup)));

            return collection;
        }
    }
}