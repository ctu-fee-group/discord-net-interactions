using System.Collections.Generic;

namespace Discord.Net.Interactions.Abstractions
{
    public interface IInteractionMatcherProvider
    {
        public IEnumerable<IInteractionMatcher> GetMatchers();
    }
}