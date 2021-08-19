namespace Discord.NET.InteractionsService.Verifier.Interfaces
{
    public interface IHasUserMessage
    {
        public IUserMessage? UserMessage { get; set; }
    }
}