using System;

namespace Discord.Net.Interactions.Abstractions
{
    public class MessageComponentInfo : InteractionInfo
    {
        public MessageComponentInfo(DiscordInteractionHandler handler, ulong? messageId = null, IUser? user = null, string? customId = null)
            : base(handler)
        {
            if (messageId is null && user is null && customId is null)
            {
                throw new ArgumentException("At least one of messageId, user, customId must be non-null");
            }
            
            MessageId = messageId;
            User = user;
            CustomId = customId;
        }

        public MessageComponentInfo(InstancedDiscordInteractionHandler instancedHandler, ulong? messageId = null, IUser? user = null, string? customId = null)
            : base(instancedHandler)
        {
            if (messageId is null && user is null && customId is null)
            {
                throw new ArgumentException("At least one of messageId, user, customId must be non-null");
            }
            
            MessageId = messageId;
            User = user;
            CustomId = customId;
        }
        
        public ulong? MessageId { get; }
        
        public IUser? User { get; }
        
        public string? CustomId { get; }
    }
}