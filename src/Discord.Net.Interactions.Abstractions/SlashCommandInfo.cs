using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Rest;
using Discord.WebSocket;

namespace Discord.Net.Interactions.Abstractions
{
    /// <summary>
    /// Handler function of slash commands without instance, <see cref="InstancedDiscordInteractionHandler"/> for
    /// command handler that can be invoked with different instances
    /// </summary>
    public delegate Task DiscordInteractionHandler(IDiscordInteraction interaction,
        CancellationToken token = new CancellationToken());

    /// <summary>
    /// Handler function of slash commands supporting invoking with different class instances
    /// for holding context per command
    /// </summary>
    public delegate Task InstancedDiscordInteractionHandler(object classInstance, IDiscordInteraction interaction,
        CancellationToken token = new CancellationToken());

    /// <summary>
    /// Information about a slash command
    /// </summary>
    public class SlashCommandInfo : InteractionInfo
    {
        public SlashCommandInfo(SlashCommandBuilder builder,
            DiscordInteractionHandler handler, bool global, ulong? guildId)
        : base(handler)
        {
            BuiltCommand = builder.Build();
            Global = global;
            GuildId = guildId;
        }
        
        public SlashCommandInfo(SlashCommandBuilder builder,
            InstancedDiscordInteractionHandler instancedHandler, bool global, ulong? guildId)
            : base (instancedHandler)
        {

            BuiltCommand = builder.Build();
            Global = global;
            GuildId = guildId;
        }

        /// <summary>
        /// If the command was registered yet
        /// </summary>
        public bool Registered { get; set; }

        /// <summary>
        /// Whether it should be a global command
        /// </summary>
        public bool Global { get; } = false;

        /// <summary>
        /// What guild to add the command to
        /// </summary>
        public ulong? GuildId { get; }

        /// <summary>
        /// Built command that is set after calling Build()
        /// </summary>
        public SlashCommandCreationProperties BuiltCommand { get; }

        /// <summary>
        /// Registered command that is set after calling RegisterCommandAsync
        /// </summary>
        public RestApplicationCommand? Command { get; set; }
    }
}