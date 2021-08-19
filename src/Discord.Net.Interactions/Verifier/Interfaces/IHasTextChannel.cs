namespace Discord.Net.Interactions.Verifier.Interfaces
{
    public interface IHasTextChannel
    {
        ITextChannel? TextChannel { get; set; }
    }
}