using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.Net.Interactions.CommandsInfo;
using Discord.Net.Interactions.Controllers.Builders;

namespace Discord.Net.Interactions.Controllers
{
    public interface IInteractionController
    {
        /// <summary>
        /// Client that is used by the interaction
        /// </summary>
        public IDiscordClient Client { get; }

        /// <summary>
        /// Context of the current interaction
        /// </summary>
        public IInteractionState Context { get; }

        /// <summary>
        /// Called when this controller is being built.
        /// Can be used to alter the process
        /// </summary>
        public void OnControllerBuilding(ControllerBuilder builder);

        /// <summary>
        /// Called when one of the interactions is being built.
        /// Can be used to alter the process
        /// </summary>
        public void OnCommandBuilding<TInfoBuilder, TInteractionInfo>(
            ControllerCommandBuilder<TInfoBuilder, TInteractionInfo> builder)
            where TInteractionInfo : InteractionInfo
            where TInfoBuilder : SlashCommandInfoBuilder<TInfoBuilder, TInteractionInfo>;

        /// <summary>
        /// Executed before command handler is executed.
        /// All information can be obtained from InteractionContext.
        /// </summary>
        /// <returns>Whether to call the handler</returns>
        public Task<bool> OnBeforeExecuting();

        /// <summary>
        /// Executed after command handler is executed.
        /// All information can be obtained from InteractionContext.
        /// </summary>
        public Task OnAfterExecuting();
    }
}