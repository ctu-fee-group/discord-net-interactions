using System;
using System.Collections.Generic;
using System.Linq;
using Discord.Net.Interactions.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Net.Interactions.DI
{
    public class DIInteractionMatcherProvider : IInteractionMatcherProvider
    {
        private readonly List<Type> _interactionMatcherTypes;
        
        public DIInteractionMatcherProvider()
        {
            _interactionMatcherTypes = new List<Type>();
        }
        
        public IServiceProvider? Provider { get; set; }

        public void RegisterInteractionMatcher(Type interactionMatcherType)
        {
            if (!interactionMatcherType.IsAssignableTo(typeof(IInteractionMatcher)))
            {
                throw new ArgumentException($@"Type {interactionMatcherType.Name} cannot be added as it doesn't inherit from IInteractionMatcher");
            }
            
            _interactionMatcherTypes.Add(interactionMatcherType);
        }

        public IEnumerable<IInteractionMatcher> GetMatchers()
        {
            if (Provider is null)
            {
                throw new InvalidOperationException("Register by type can only be done if service provider is used");
            }

            return _interactionMatcherTypes
                .Select(x => Provider.GetRequiredService(x))
                .Cast<IInteractionMatcher>();
        }
    }
}