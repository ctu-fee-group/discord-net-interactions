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
    public delegate Task DiscordInteractionHandler(SocketInteraction interaction,
        CancellationToken token = new CancellationToken());

    /// <summary>
    /// Handler function of slash commands supporting invoking with different class instances
    /// for holding context per command
    /// </summary>
    public delegate Task InstancedDiscordInteractionHandler(object classInstance, SocketInteraction interaction,
        CancellationToken token = new CancellationToken());

    /// <summary>
    /// Information about a slash command
    /// </summary>
    public class SlashCommandInfo : InteractionInfo
    {
        public SlashCommandInfo(SlashCommandBuilder builder,
            DiscordInteractionHandler handler, bool global)
        : base(handler)
        {
            Name = builder.Name;
            BuiltCommand = builder.Build();
            Global = global;
        }
        
        public SlashCommandInfo(SlashCommandBuilder builder,
            InstancedDiscordInteractionHandler instancedHandler, bool global)
            : base (instancedHandler)
        {
            Name = builder.Name;
            BuiltCommand = builder.Build();
            Global = global;
        }
        
        public string Name { get; }

        /// <summary>
        /// Whether it should be a global command
        /// </summary>
        public bool Global { get; } = false;

        /// <summary>
        /// Built command that is set after calling Build()
        /// </summary>
        public SlashCommandProperties BuiltCommand { get; }
    }
}