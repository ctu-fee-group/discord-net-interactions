namespace Discord.Net.Interactions.Verifier.Interfaces
{
    public interface IHasUserMessage
    {
        public IUserMessage? UserMessage { get; set; }
    }
}