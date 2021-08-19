using System.Threading.Tasks;
using Discord.NET.InteractionsService.Verifier.Interfaces;

namespace Discord.NET.InteractionsService.Verifier.Verifiers
{
    public static class EmojiVerifierExtensions
    {
        public static CommandVerifier<T> VerifyEmote<T>(this CommandVerifier<T> verifier, string emojiString,
            string parameterName = "emoji")
            where T : class, IHasEmote, new()
        {
            verifier.QueueWork(() => verifier.VerifyEmoteAsync(emojiString, parameterName));
            return verifier;
        }

        private static Task<CommandVerifier<T>> VerifyEmoteAsync<T>(this CommandVerifier<T> verifier,
            string emojiString, string parameterName = "emoji")
            where T : class, IHasEmote, new()
        {
            IEmote? outEmote = null;
            if (outEmote == null && Emote.TryParse(emojiString, out Emote emote))
            {
                outEmote = emote;
            }
            
            if (outEmote == null && Emoji.TryParse(emojiString, out Emoji emoji))
            {
                outEmote = emoji;
            }

            if (outEmote == null)
            {
                verifier.SetFailed(parameterName, "Emoji cannot be parsed.");
                return Task.FromResult(verifier);
            }

            verifier.Result.Emote = outEmote;

            return Task.FromResult(verifier);
        }
    }
}