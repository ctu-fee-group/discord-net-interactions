using System;

namespace Discord.Net.Interactions.Controllers
{
    public interface IInteractionControllerInfo : IDisposable
    {
        public IInteractionController Controller { get; }
    }
}