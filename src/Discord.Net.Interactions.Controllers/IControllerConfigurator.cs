using System.Threading;
using Discord.Net.Interactions.Abstractions;
using Discord.WebSocket;

namespace Discord.Net.Interactions.Controllers
{
    public interface IControllerConfigurator
    {
        public void FillControllerContext(IInteractionControllerInfo controller, IDiscordClient client,
            SocketInteraction interaction, InteractionInfo info, CancellationToken token);
    }
}