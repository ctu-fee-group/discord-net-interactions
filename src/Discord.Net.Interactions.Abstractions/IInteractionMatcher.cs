namespace Discord.Net.Interactions.Abstractions
{
    public interface IInteractionMatcher
    {
        public bool Matches(IDiscordInteraction discordInteraction, InteractionInfo info);
    }
}