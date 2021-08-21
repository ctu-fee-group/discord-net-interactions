using Discord.Net.Interactions.Abstractions;
using Discord.WebSocket;

namespace Discord.Net.Interactions.InteractionMatchers
{
    public class SlashCommandMatcher : IInteractionMatcher
    {
        public bool Matches(IDiscordInteraction discordInteraction, InteractionInfo info)
        {
            if (discordInteraction is not SocketSlashCommand command)
            {
                return false;
            }

            if (info is not SlashCommandInfo commandInfo)
            {
                return false;
            }

            return command.Data.Name == commandInfo.BuiltCommand.Name;
        }
    }
}