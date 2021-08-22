using Discord.Net.Interactions.Abstractions;
using Discord.WebSocket;

namespace Discord.Net.Interactions.InteractionMatchers
{
    public class MessageComponentMatcher : IInteractionMatcher
    {
        public bool Matches(IDiscordInteraction discordInteraction, InteractionInfo info)
        {
            if (discordInteraction is not SocketMessageComponent component)
            {
                return false;
            }

            if (info is not MessageComponentInfo componentInfo)
            {
                return false;
            }

            return (componentInfo.MessageId is null || componentInfo.MessageId == component.Message.Id) &&
                   (componentInfo.CustomId is null || componentInfo.CustomId == component.Data.CustomId) &&
                   (componentInfo.User is null || componentInfo.User.Id == component.User.Id);
        }
    }
}