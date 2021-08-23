using System;
using System.Collections.Generic;
using Discord.Net.Interactions.Abstractions;
using Discord.WebSocket;
using Microsoft.VisualBasic;

namespace Discord.Net.Interactions.Controllers
{
    public interface IControllerResolver
    {
        public void RegisterControllerInteraction(Type controllerType, InteractionInfo interactionInfo);
        public void UnregisterControllerInteraction(InteractionInfo interactionInfo);
        
        public IEnumerable<IInteractionControllerInfo> GetControllers();
        public IEnumerable<Type> GetControllerTypes();
        public IInteractionControllerInfo ResolveController(InteractionInfo interactionInfo); 
    }
}