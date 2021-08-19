namespace Discord.NET.InteractionsService.Verifier.Interfaces
{
    public interface IHasTextChannel
    {
        ITextChannel? TextChannel { get; set; }
    }
}