using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Rest;
using Discord.WebSocket;

namespace Discord.Net.Interactions.Abstractions
{
    public static class InteractionExtensions
    {
        public static int MaxMessageLength = 2000;
        
        public static IEnumerable<string> Chunk(this string? str, int chunkSize) =>
            !string.IsNullOrEmpty(str) ?
                Enumerable.Range(0, (int)Math.Ceiling(((double)str.Length) / chunkSize))
                    .Select(i => str
                        .Substring(i * chunkSize,
                            (i * chunkSize + chunkSize <= str.Length) ? chunkSize : str.Length - i * chunkSize))
                : Enumerable.Empty<string>();
        
        /// <summary>
        /// Call RespondAsync, but split it to 2000 character messages to prevent from messages not being sent for being too long
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="text"></param>
        /// <param name="embeds"></param>
        /// <param name="isTTS"></param>
        /// <param name="ephemeral"></param>
        /// <param name="allowedMentions"></param>
        /// <param name="options"></param>
        /// <param name="component"></param>
        public static async Task RespondChunkAsync(this SocketInteraction interaction, string? text = null, Embed[]? embeds = null, bool isTTS = false,
            bool ephemeral = false, AllowedMentions? allowedMentions = null, RequestOptions? options = null, MessageComponent? component = null)
        {
            IEnumerable<string> chunked = text.Chunk(MaxMessageLength).ToList();

            await interaction
                .RespondAsync(text: chunked.FirstOrDefault(), embeds, isTTS, ephemeral, allowedMentions, options,
                    component);
            
            foreach (string chunk in chunked.Skip(1))
            {
                await interaction
                    .FollowupAsync(chunk, null, isTTS, ephemeral, allowedMentions, options, null);
            }
        }
        
        /// <summary>
        /// Call FollowupAsync, but split it to 2000 character messages to prevent from messages not being sent for being too long
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="text"></param>
        /// <param name="embeds"></param>
        /// <param name="isTTS"></param>
        /// <param name="ephemeral"></param>
        /// <param name="allowedMentions"></param>
        /// <param name="options"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        public static async Task<RestFollowupMessage> FollowupChunkAsync(this SocketInteraction interaction, string? text = null, Embed[]? embeds = null, bool isTTS = false,
            bool ephemeral = false, AllowedMentions? allowedMentions = null, RequestOptions? options = null, MessageComponent? component = null)
        {
            List<string> chunked = text.Chunk(MaxMessageLength).ToList();

            RestFollowupMessage last;
            last = await interaction
                .FollowupAsync(text: chunked.FirstOrDefault(), embeds, isTTS, ephemeral, allowedMentions, options,
                    component);
            
            foreach (string chunk in chunked.Skip(1))
            {
                last = await interaction
                    .FollowupAsync(chunk, null, isTTS, ephemeral, allowedMentions, options, null);
            }

            return last;
        }
    }
}