using System.Collections.Generic;

namespace Discord.Net.Interactions.Abstractions
{
    /// <summary>
    /// Provider ICommandGroup for ICommandRegistrator
    /// </summary>
    public interface ICommandsGroupProvider
    {
        public IEnumerable<ICommandGroup> GetGroups();
    }
}