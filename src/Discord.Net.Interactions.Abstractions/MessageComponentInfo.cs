namespace Discord.Net.Interactions.Abstractions
{
    public class MessageComponentInfo : InteractionInfo
    {
        public MessageComponentInfo(DiscordInteractionHandler handler, ulong messageId, IUser? user = null, string? customId = null)
            : base(handler)
        {
            MessageId = messageId;
            User = user;
            CustomId = customId;
        }

        public MessageComponentInfo(InstancedDiscordInteractionHandler instancedHandler, ulong messageId, IUser? user = null, string? customId = null)
            : base(instancedHandler)
        {
            MessageId = messageId;
            User = user;
            CustomId = customId;
        }
        
        public ulong MessageId { get; }
        
        public IUser? User { get; }
        
        public string? CustomId { get; }
    }
}