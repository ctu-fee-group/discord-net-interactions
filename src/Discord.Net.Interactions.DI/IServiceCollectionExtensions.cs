using System;
using Discord.Net.Interactions.Abstractions;
using Discord.Net.Interactions.Commands;
using Discord.Net.Interactions.Handlers;
using Discord.Net.Interactions.InteractionMatchers;
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
        /// <typeparam name="TInteractionInfo"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddEveryoneCommandPermissionResolver<TInteractionInfo>(
            this IServiceCollection collection)
            where TInteractionInfo : InteractionInfo
        {
            return collection
                .AddSingleton<ICommandPermissionsResolver<TInteractionInfo>,
                    EveryoneCommandPermissionResolver<TInteractionInfo>>();
        }
        
        /// <summary>
        /// Add bulk command registrator <see cref="BulkCommandsRegistrator{TInteractionInfo}"/>>
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="TInteractionInfo"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddBulkCommandRegistrator<TInteractionInfo>(
            this IServiceCollection collection)
            where TInteractionInfo : SlashCommandInfo
        {
            return collection
                .AddSingleton<ICommandsRegistrator, BulkCommandsRegistrator<TInteractionInfo>>();
        }
        
        /// <summary>
        /// Add one by one command registrator <see cref="AddOneByOneCommandRegistrator{TInteractionInfo}"/>>
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="TInteractionInfo"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddOneByOneCommandRegistrator<TInteractionInfo>(
            this IServiceCollection collection)
            where TInteractionInfo : SlashCommandInfo
        {
            return collection
                .AddSingleton<ICommandsRegistrator, OneByOneCommandsRegistrator<TInteractionInfo>>();
        }

        /// <summary>
        /// Adds default InteractionHandler to the service collection
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="configure">Function that is to make code more readable, it is invoken with the same collection that was passed to the function. It should register CommandGroups</param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultInteractionService<TInteractionInfo>(this IServiceCollection collection,
            Action<IServiceCollection>? configure = null)
            where TInteractionInfo : InteractionInfo
        {
            collection
                .AddOptions<DICommandGroupsProvider>();
            collection
                .AddOptions<DIInteractionMatcherProvider>();

            collection
                .AddSingleton<ICommandsGroupProvider>(p =>
                {
                    DICommandGroupsProvider provider =
                        p.GetRequiredService<IOptions<DICommandGroupsProvider>>().Value;
                    provider.Provider = p;

                    return provider;
                })
                .AddSingleton<IInteractionMatcherProvider>(p =>
                {
                    DIInteractionMatcherProvider provider =
                        p.GetRequiredService<IOptions<DIInteractionMatcherProvider>>().Value;
                    provider.Provider = p;

                    return provider;
                })
                .AddSingleton<IInteractionHolder, ThreadSafeInteractionHolder>()
                .AddSingleton<InteractionHandler>()
                .AddSingleton<InteractionsService<TInteractionInfo>>();

            collection
                .AddInteractionMatcher<SlashCommandMatcher>()
                .AddInteractionMatcher<MessageComponentMatcher>();

            configure?.Invoke(collection);

            return collection;
        }

        /// <summary>
        /// Adds CommandGroup as singleton and configures DICommandGroupsProvider to provide this group command correctly
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IServiceCollection AddCommandGroup<TGroup>(this IServiceCollection collection)
            where TGroup : class, ICommandGroup
        {
            collection.AddSingleton<TGroup>();

            collection.Configure<DICommandGroupsProvider>(handler =>
                handler.RegisterGroupType(typeof(TGroup)));

            return collection;
        }
        
        /// <summary>
        /// Adds CommandGroup as scoped and configures DICommandGroupsProvider to provide this group command correctly
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IServiceCollection AddScopedCommandGroup<TGroup>(this IServiceCollection collection)
            where TGroup : class, ICommandGroup
        {
            collection.AddScoped<TGroup>();

            collection.Configure<DICommandGroupsProvider>(handler =>
                handler.RegisterGroupType(typeof(TGroup)));

            return collection;
        }
        
        /// <summary>
        /// Adds CommandGroup as singleton and configures DICommandGroupsProvider to provide this group command correctly
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IServiceCollection AddInteractionMatcher<TInteractionMatcher>(this IServiceCollection collection)
            where TInteractionMatcher : class, IInteractionMatcher
        {
            collection.AddScoped<TInteractionMatcher>();

            collection.Configure<DIInteractionMatcherProvider>(handler =>
                handler.RegisterInteractionMatcher(typeof(TInteractionMatcher)));

            return collection;
        }
    }
}