using System;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Discord.Net.Interactions.Components
{
    public static class SocketInteractionExtensions
    {
        /// <summary>
        /// Send message with MessageComponent using button choices as respond to the interaction
        ///
        /// Can be used for yes, no etc.
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="text"></param>
        /// <param name="embeds"></param>
        /// <param name="isTTS"></param>
        /// <param name="ephemeral"></param>
        /// <param name="allowedMentions"></param>
        /// <param name="options"></param>
        /// <param name="choices">Choices in format (label, custom id)</param>
        /// <returns></returns>
        public static Task<IMessage> RespondChoiceAsync(this SocketInteraction interaction, string? text = null,
            Discord.Embed[]? embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            Discord.AllowedMentions? allowedMentions = null,
            RequestOptions? options = null,
            params (string, string)[] choices)
        {
            return interaction.SendChoiceAsync(async (component) =>
            {
                await interaction.RespondAsync(text, embeds, isTTS, ephemeral, allowedMentions, options, component);
                return await interaction.GetOriginalResponseAsync();
            }, choices);
        }

        /// <summary>
        /// Send message with MessageComponent using button choices as followup of the interaction
        ///
        /// Can be used for yes, no etc.
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="text"></param>
        /// <param name="embeds"></param>
        /// <param name="isTTS"></param>
        /// <param name="ephemeral"></param>
        /// <param name="allowedMentions"></param>
        /// <param name="options"></param>
        /// <param name="choices">Choices in format (label, custom id)</param>
        /// <returns></returns>
        public static Task<IMessage> FollowupChoiceAsync(this SocketInteraction interaction, string? text = null,
            Discord.Embed[]? embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            Discord.AllowedMentions? allowedMentions = null,
            RequestOptions? options = null,
            params (string, string)[] choices)
        {
            return interaction.SendChoiceAsync(
                async (component) =>
                    await interaction.FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, options, component),
                choices);
        }

        private static Task<IMessage> SendChoiceAsync(this SocketInteraction interaction,
            Func<MessageComponent, Task<IMessage>> sendAction, params (string, string)[] choices)
        {
            ComponentBuilder builder = new ComponentBuilder();

            foreach ((string, string) choice in choices)
            {
                builder.WithButton(choice.Item1, choice.Item2);
            }

            return sendAction(builder.Build());
        }
    }
}