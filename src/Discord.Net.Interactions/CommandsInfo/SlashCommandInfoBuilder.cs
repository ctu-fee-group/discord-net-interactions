using System;
using Discord.Net.Interactions.Abstractions;

namespace Discord.Net.Interactions.CommandsInfo
{
    public sealed class SlashCommandInfoBuilder
        : SlashCommandInfoBuilder<SlashCommandInfoBuilder, SlashCommandInfo>
    {
        public override SlashCommandInfo Build()
        {
            if (DiscordNetBuilder is null)
            {
                throw new InvalidOperationException("DiscordNetBuilder must be set");
            }

            SlashCommandInfo info;
            if (Handler is not null)
            {
                info = new SlashCommandInfo(DiscordNetBuilder, Handler, Global);
            }
            else if (InstancedHandler is not null)
            {
                info = new SlashCommandInfo(DiscordNetBuilder, InstancedHandler, Global);
            }
            else
            {
                throw new InvalidOperationException("At least one of Handler, InstancedHandler must be set");
            }

            return info;
        }
    }

    public abstract class SlashCommandInfoBuilder<TBuilder, TInteractionInfo>
        where TInteractionInfo : InteractionInfo
        where TBuilder : SlashCommandInfoBuilder<TBuilder, TInteractionInfo>
    {
        private readonly TBuilder _builderInstance;

        protected SlashCommandInfoBuilder()
        {
            _builderInstance = (TBuilder)this;
        }

        /// <summary>
        /// If the command should be registered as global
        /// </summary>
        public bool Global { get; set; }

        /// <summary>
        /// Handler that will be called when the command was executed by a user
        /// </summary>
        public DiscordInteractionHandler? Handler { get; set; }

        /// <summary>
        /// Handler that will be called when the command was executed by a user
        /// </summary>
        public InstancedDiscordInteractionHandler? InstancedHandler { get; set; }

        /// <summary>
        /// Builder used to build SlashCommandCreationOptions
        /// </summary>
        public SlashCommandBuilder? DiscordNetBuilder { get; set; }

        /// <summary>
        /// Set SlashCommandCreationOptions builder
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public TBuilder WithBuilder(SlashCommandBuilder builder)
        {
            DiscordNetBuilder = builder;
            return _builderInstance;
        }

        /// <summary>
        /// Set command handler deleagate to be called when handling the command
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public TBuilder WithHandler(DiscordInteractionHandler handler)
        {
            Handler = handler;
            return _builderInstance;
        }

        /// <summary>
        /// Set command handler deleagate to be called when handling the command
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public TBuilder WithHandler(InstancedDiscordInteractionHandler handler)
        {
            InstancedHandler = handler;
            return _builderInstance;
        }

        /// <summary>
        /// Set whether the command should be global or guild
        /// </summary>
        /// <param name="global"></param>
        /// <returns></returns>
        public TBuilder SetGlobal(bool global = true)
        {
            Global = global;
            return _builderInstance;
        }

        /// <summary>
        /// Build SlashCommandInfo
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public abstract TInteractionInfo Build();
    }
}