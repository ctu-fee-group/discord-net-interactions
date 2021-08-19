using System.Threading;
using System.Threading.Tasks;
using Discord.Rest;
using Discord.WebSocket;

namespace Discord.Net.Interactions.Abstractions
{
    public delegate Task SlashCommandHandler(SocketSlashCommand command, CancellationToken token = new CancellationToken());

    /// <summary>
    /// Information about a slash command
    /// </summary>
    public class SlashCommandInfo
    {
        public SlashCommandInfo(SlashCommandBuilder builder,
            SlashCommandHandler handler, bool global, ulong? guildId)
        {
            BuiltCommand = builder.Build();
            Handler = handler;
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
        /// What handler to execute when command execution is requested
        /// </summary>
        public SlashCommandHandler Handler { get; }

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