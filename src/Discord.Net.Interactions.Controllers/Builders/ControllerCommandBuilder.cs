using Discord.Net.Interactions.Abstractions;
using Discord.Net.Interactions.CommandsInfo;

namespace Discord.Net.Interactions.Controllers.Builders
{
    public class ControllerCommandBuilder<TInfoBuilder, TInteractionInfo>
        where TInteractionInfo : InteractionInfo
        where TInfoBuilder : SlashCommandInfoBuilder<TInfoBuilder, TInteractionInfo>
    {
        public SlashCommandBuilder SlashBuilder { get; }
        
        public SlashCommandInfoBuilder<TInfoBuilder, TInteractionInfo> InfoBuilder { get; }
    }
}