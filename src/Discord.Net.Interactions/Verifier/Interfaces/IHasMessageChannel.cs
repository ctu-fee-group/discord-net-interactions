namespace Discord.Net.Interactions.Verifier.Interfaces
{
    public interface IHasMessageChannel
    {
        public IMessageChannel? Channel { get; set; }
    }
}