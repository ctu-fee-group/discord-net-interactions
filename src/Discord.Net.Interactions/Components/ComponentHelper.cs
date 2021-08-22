using System;
using System.Collections.Generic;
using System.Linq;
using Discord.Net.Interactions.Abstractions;
using Discord.Net.Interactions.Executors;

namespace Discord.Net.Interactions.Components
{
    public class ComponentHelper : IComponentHelper
    {
        private readonly IInteractionHolder _holder;

        public ComponentHelper(IInteractionHolder holder)
        {
            _holder = holder;
        }

        public void ForMessage(IMessage message, IUser? user,
            Action<InteractionExecutorBuilder<MessageComponentInfo>> builderAction,
            params (string, DiscordInteractionHandler)[] infos)
        {
            InteractionExecutorBuilder<MessageComponentInfo> executorBuilder =
                new InteractionExecutorBuilder<MessageComponentInfo>();
            builderAction(executorBuilder);

            IInteractionExecutor executor = executorBuilder.Build();

            foreach ((string, DiscordInteractionHandler) info in infos)
            {
                MessageComponentInfo componentInfo = new MessageComponentInfo(info.Item2, message.Id, user, info.Item1);
                _holder.AddInteraction(componentInfo, executor);
            }
        }

        public void ForMessageChoice(IMessage message, IUser? user,
            Action<InteractionExecutorBuilder<MessageComponentInfo>> builderAction,
            params (string, DiscordInteractionHandler)[] infos)
        {
            List<MessageComponentInfo> componentInfos = new List<MessageComponentInfo>();
            foreach ((string, DiscordInteractionHandler) info in infos)
            {
                MessageComponentInfo componentInfo = new MessageComponentInfo(info.Item2, message.Id, user, info.Item1);
                componentInfos.Add(componentInfo);
            }

            InteractionExecutorBuilder<MessageComponentInfo> executorBuilder =
                new InteractionExecutorBuilder<MessageComponentInfo>();
            builderAction(executorBuilder);

            IInteractionExecutor executor = executorBuilder
                .WithRemoveInteractions(_holder, componentInfos.Cast<InteractionInfo>().ToArray())
                .Build();

            foreach (MessageComponentInfo componentInfo in componentInfos)
            {
                _holder.AddInteraction(componentInfo, executor);
            }
        }
    }
}