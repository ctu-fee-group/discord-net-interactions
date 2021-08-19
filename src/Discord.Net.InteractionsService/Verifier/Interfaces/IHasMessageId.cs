namespace Discord.NET.InteractionsService.Verifier.Interfaces
{
    public interface IHasMessageId
    {
        public ulong? MessageId { get; set; }
    }
}