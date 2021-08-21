namespace Discord.Net.Interactions.Abstractions
{
    public abstract class InteractionInfo
    {
        protected InteractionInfo(DiscordInteractionHandler handler)
        {
            Handler = handler;
        }

        protected InteractionInfo(InstancedDiscordInteractionHandler instancedHandler)
        {
            InstancedHandler = instancedHandler;
        }

        /// <summary>
        /// What handler to execute when command execution is requested
        /// </summary>
        public DiscordInteractionHandler? Handler { get; }
        
        /// <summary>
        /// What handler to execute when command execution is requested
        /// </summary>
        public InstancedDiscordInteractionHandler? InstancedHandler { get; }
    }
}