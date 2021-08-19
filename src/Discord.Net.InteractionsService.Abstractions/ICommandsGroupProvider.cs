using System.Collections.Generic;

namespace Discord.NET.InteractionsService.Abstractions
{
    /// <summary>
    /// Provider ICommandGroup for ICommandRegistrator
    /// </summary>
    public interface ICommandsGroupProvider
    {
        public IEnumerable<ICommandGroup> GetGroups();
    }
}