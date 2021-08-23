using System;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.Net.Interactions.CommandsInfo;
using Discord.Net.Interactions.Controllers.Builders;
using Discord.Rest;

namespace Discord.Net.Interactions.Controllers
{
    public abstract class InteractionController : IInteractionController
    {
        private IDiscordClient? _client;
        private IInteractionState? _state;

        public IDiscordClient Client
        {
            get
            {
                if (_client is null)
                {
                    throw new InvalidOperationException("The client is null");
                }

                return _client;
            }
        }

        public IInteractionState Context
        {
            get
            {
                if (_state is null)
                {
                    throw new InvalidOperationException("The context is null");
                }

                return _state;
            }
        }

        public virtual void OnControllerBuilding(ControllerBuilder builder)
        {
        }

        public virtual void OnCommandBuilding<TInfoBuilder, TInteractionInfo>(ControllerCommandBuilder<TInfoBuilder, TInteractionInfo> builder) where TInfoBuilder : SlashCommandInfoBuilder<TInfoBuilder, TInteractionInfo> where TInteractionInfo : InteractionInfo
        {
        }
        
        public virtual Task<bool> OnBeforeExecuting() => Task.FromResult(true);

        public virtual Task OnAfterExecuting() => Task.CompletedTask;

        internal void SetContext(IInteractionState state)
        {
            _state = state;
        }

        internal void SetClient(IDiscordClient client)
        {
            _client = client;
        }
    }
}