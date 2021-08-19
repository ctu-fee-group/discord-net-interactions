namespace Discord.Net.Interactions.Verifier.Interfaces
{
    public interface IHasMessageId
    {
        public ulong? MessageId { get; set; }
    }
}