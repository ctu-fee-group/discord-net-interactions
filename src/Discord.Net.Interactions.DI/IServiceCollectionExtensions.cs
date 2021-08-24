using System;
using Discord.Net.Interactions.Abstractions;
using Discord.Net.Interactions.Commands;
using Discord.Net.Interactions.Components;
using Discord.Net.Interactions.Handlers;
using Discord.Net.Interactions.InteractionMatchers;
using Discord.Net.Interactions.Permissions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Discord.Net.Interactions.DI
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add guild resolver for specified guild
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="configuration">Configuration with <see cref="GuildOptions"/> schema</param>
        /// <typeparam name="TInteractionInfo"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddOneGuildResolver<TInteractionInfo>(
            this IServiceCollection collection, IConfiguration configuration)
            where TInteractionInfo : SlashCommandInfo
        {
            collection
                .Configure<GuildOptions>(configuration);
            
            return collection
                .AddSingleton<IGuildResolver<TInteractionInfo>, DIOneGuildResolver<TInteractionInfo>>();
        }

        
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

        public static IServiceCollection AddInteractionsProvider<T>(this IServiceCollection collection)
            where T : class
        {
            collection
                .AddOptions<DIProvider<T>>();

            return collection
                .AddSingleton<IProvider<T>>(p =>
                {
                    DIProvider<T> provider = p.GetRequiredService<IOptions<DIProvider<T>>>().Value;
                    provider.Provider = p;

                    return provider;
                });
        }

        /// <summary>
        /// Adds default InteractionHandler to the service collection
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="configure">Function that is to make code more readable, it is invoken with the same collection that was passed to the function. It should register CommandGroups</param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultInteractionService(this IServiceCollection collection,
            Action<IServiceCollection>? configure = null)
        {
            collection
                .AddSingleton<IInteractionHolder, ThreadSafeInteractionHolder>()
                .AddSingleton<IComponentHelper, ComponentHelper>()
                .AddSingleton<InteractionHandler>()
                .AddSingleton<InteractionsService>();

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
            return collection.AddProviderValue<ICommandGroup, TGroup>();

        }


        /// <summary>
        /// Adds CommandGroup as singleton and configures DICommandGroupsProvider to provide this group command correctly
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IServiceCollection AddInteractionMatcher<TInteractionMatcher>(this IServiceCollection collection)
            where TInteractionMatcher : class, IInteractionMatcher
        {
            return collection.AddProviderValue<IInteractionMatcher, TInteractionMatcher>();
        }
        
        public static IServiceCollection AddProviderValue<TProviderType, TProvideValueType>(this IServiceCollection collection)
            where TProvideValueType : class, TProviderType
            where TProviderType : class
        {
            collection.AddInteractionsProvider<TProviderType>();
            collection.AddSingleton<TProvideValueType>();

            collection.Configure<DIProvider<TProviderType>>(handler =>
                handler.RegisterType(typeof(TProvideValueType)));

            return collection;
        }
    }
}