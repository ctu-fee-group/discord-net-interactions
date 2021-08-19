using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Christofel.BaseLib.Database.Models;
using Christofel.BaseLib.Database.Models.Enums;
using Christofel.BaseLib.Extensions;
using Christofel.BaseLib.Permissions;
using Christofel.CommandsLib.Extensions;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace Christofel.CommandsLib.CommandsInfo
{
    public delegate Task SlashCommandHandler(SocketSlashCommand command, CancellationToken token = new CancellationToken());

    /// <summary>
    /// Information about a slash command
    /// </summary>
    public class SlashCommandInfo
    {
        public SlashCommandInfo(SlashCommandBuilder builder,
            SlashCommandHandler handler)
        {
            BuiltCommand = builder.Build();
            Handler = handler;
        }
        
        /// <summary>
        /// If the command was registered yet
        /// </summary>
        public bool Registered { get; set; }

        /// <summary>
        /// Whether it should be a global command
        /// </summary>
        public bool Global { get; set; } = false;
        
        /// <summary>
        /// What guild to add the command to
        /// </summary>
        public ulong? GuildId { get; set; }
        
        /// <summary>
        /// What handler to execute when command execution is requested
        /// </summary>
        public SlashCommandHandler Handler { get; }

        /// <summary>
        /// Built command that is set after calling Build()
        /// </summary>
        public SlashCommandCreationProperties BuiltCommand { get; private set; }
        
        /// <summary>
        /// Registered command that is set after calling RegisterCommandAsync
        /// </summary>
        public RestApplicationCommand? Command { get; set; }
    }
}