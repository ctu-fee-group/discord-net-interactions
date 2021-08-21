using Discord.WebSocket;

namespace Discord.Net.Interactions.Abstractions
{
    public static class DiscordInteractionExtensions
    {
        public static string GetName(this IDiscordInteraction interaction) =>
            interaction switch
            {
                SocketSlashCommand command => $"/{command.Data.Name}",
                SocketMessageComponent component => $"MessageComponent on message {component.Message.Id} with custom id {component.Data.CustomId}",
                _ => "Unknown name"
            };

        public static string GetUser(this IDiscordInteraction interaction) =>
            interaction switch
            {
                SocketInteraction socketInteraction => $"{socketInteraction.User.Mention} ({socketInteraction.User})",
                _ => "Unknown user",
            };
    }
}