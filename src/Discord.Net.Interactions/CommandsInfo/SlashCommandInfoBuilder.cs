using System;
using Discord.Net.Interactions.Abstractions;

namespace Discord.Net.Interactions.CommandsInfo
{
    public sealed class SlashCommandInfoBuilder
        : SlashCommandInfoBuilder<SlashCommandInfoBuilder, SlashCommandInfo>
    {
        public override SlashCommandInfo Build()
        {
            if (DiscordNetBuilder == null || Handler == null)
            {
                throw new InvalidOperationException("DiscordNetBuilder, Permission and Handler must be set");
            }

            SlashCommandInfo info = new SlashCommandInfo(DiscordNetBuilder, Handler, Global, GuildId);

            return info;
        }
    }

    public abstract class SlashCommandInfoBuilder<TBuilder, TSlashInfo>
        where TSlashInfo : SlashCommandInfo
        where TBuilder : SlashCommandInfoBuilder<TBuilder, TSlashInfo>
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
        /// Where the command should be added to
        /// </summary>
        public ulong? GuildId { get; set; }

        /// <summary>
        /// Handler that will be called when the command was executed by a user
        /// </summary>
        public SlashCommandHandler? Handler { get; set; }

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
        public TBuilder WithHandler(SlashCommandHandler handler)
        {
            Handler = handler;
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
        /// Set Christofel permission for the command
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public TBuilder WithPermission(string permission)
        {
            Permission = permission;
            return _builderInstance;
        }

        /// <summary>
        /// Set guild for the guild command to be added to, set the command non-global (guild)
        /// </summary>
        /// <param name="guildId"></param>
        /// <returns></returns>
        public TBuilder WithGuild(ulong guildId)
        {
            GuildId = guildId;
            Global = false;
            return _builderInstance;
        }

        /// <summary>
        /// Build SlashCommandInfo
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public abstract TSlashInfo Build();
    }
}