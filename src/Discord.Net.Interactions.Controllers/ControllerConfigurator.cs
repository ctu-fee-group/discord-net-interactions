using System;
using System.Threading;
using Discord.Net.Interactions.Abstractions;
using Discord.WebSocket;

namespace Discord.Net.Interactions.Controllers
{
    public class ControllerConfigurator : IControllerConfigurator
    {
        public void FillControllerContext(IInteractionControllerInfo controllerInfo, IDiscordClient client,
            SocketInteraction interaction,
            InteractionInfo info, CancellationToken token)
        {
            if (controllerInfo.Controller is not InteractionController interactionController)
            {
                throw new InvalidOperationException(
                    $"Supported controller types must inherit from {typeof(InteractionController).FullName}");
            }

            IGuild? guild = (interaction.Channel as IGuildChannel)?.Guild;

            interactionController.SetClient(client);
            interactionController.SetContext(new InteractionContext(interaction.User, interaction.Channel, guild, info,
                interaction, token));
        }

        private class InteractionContext : IInteractionState
        {
            public InteractionContext(IUser user, IChannel? channel, IGuild? guild, InteractionInfo interactionInfo,
                SocketInteraction interaction, CancellationToken cancellationToken)
            {
                User = user;
                Channel = channel;
                Guild = guild;
                InteractionInfo = interactionInfo;
                Interaction = interaction;
                CancellationToken = cancellationToken;
            }

            public IUser User { get; }
            public IChannel? Channel { get; }
            public IGuild? Guild { get; }
            public InteractionInfo InteractionInfo { get; }
            public SocketInteraction Interaction { get; }
            public CancellationToken CancellationToken { get; }
        }
    }
}