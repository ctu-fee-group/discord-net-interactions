using System;
using Discord.Net.Interactions.Abstractions;
using Discord.Net.Interactions.Commands;
using Discord.Net.Interactions.Handlers;
using Discord.Net.Interactions.Permissions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Discord.Net.Interactions.DI
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add permission resolver that will simply allow execution of the command to everyone
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="TSlashInfo"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddEveryoneCommandPermissionResolver<TSlashInfo>(
            this IServiceCollection collection)
            where TSlashInfo : SlashCommandInfo
        {
            return collection
                .AddSingleton<ICommandPermissionsResolver<TSlashInfo>,
                    EveryoneCommandPermissionResolver<TSlashInfo>>();
        }
        
        /// <summary>
        /// Add bulk command registrator <see cref="BulkCommandsRegistrator{TSlashInfo}"/>>
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="TSlashInfo"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddBulkCommandRegistrator<TSlashInfo>(
            this IServiceCollection collection)
            where TSlashInfo : SlashCommandInfo
        {
            return collection
                .AddSingleton<ICommandsRegistrator<TSlashInfo>, BulkCommandsRegistrator<TSlashInfo>>();
        }
        
        /// <summary>
        /// Add one by one command registrator <see cref="AddOneByOneCommandRegistrator{TSlashInfo}"/>>
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="TSlashInfo"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddOneByOneCommandRegistrator<TSlashInfo>(
            this IServiceCollection collection)
            where TSlashInfo : SlashCommandInfo
        {
            return collection
                .AddSingleton<ICommandsRegistrator<TSlashInfo>, OneByOneCommandsRegistrator<TSlashInfo>>();
        }

        /// <summary>
        /// Adds default InteractionHandler to the service collection
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="configure">Function that is to make code more readable, it is invoken with the same collection that was passed to the function. It should register CommandGroups</param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultInteractionService<TSlashInfo>(this IServiceCollection collection,
            Action<IServiceCollection>? configure = null)
            where TSlashInfo : SlashCommandInfo
        {
            collection
                .AddOptions<DICommandGroupsProvider<TSlashInfo>>();

            collection
                .AddSingleton<ICommandsGroupProvider<TSlashInfo>>(p =>
                {
                    DICommandGroupsProvider<TSlashInfo> provider =
                        p.GetRequiredService<IOptions<DICommandGroupsProvider<TSlashInfo>>>().Value;
                    provider.Provider = p;

                    return provider;
                })
                .AddSingleton<ICommandHolder<TSlashInfo>, ThreadSafeCommandHolder<TSlashInfo>>()
                .AddSingleton<InteractionHandler<TSlashInfo>>()
                .AddSingleton<InteractionsService<TSlashInfo>>();

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