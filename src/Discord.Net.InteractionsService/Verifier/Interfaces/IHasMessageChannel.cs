namespace Discord.NET.InteractionsService.Verifier.Interfaces
{
    public interface IHasMessageChannel
    {
        public IMessageChannel? Channel { get; set; }
    }
}