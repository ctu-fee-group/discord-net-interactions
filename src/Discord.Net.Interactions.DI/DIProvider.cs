using System;
using System.Collections.Generic;
using System.Linq;
using Discord.Net.Interactions.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Net.Interactions.DI
{
    /// <summary>
    /// Service for providing ICommandGroup.
    /// </summary>
    /// <remarks>
    /// Command groups should be registered using RegisterGroup.
    /// For DI, RegisterGroupType can be used. With DI Options should be used.
    /// CommandGroupsService should be treated as IOptions and it should be configured using
    /// <c>IServiceCollection.Configure<CommandsGroupsService>((groupsService) => groupsService.RegisterGroupType(...))</c>
    /// </remarks>
    public class DIProvider<T> : IProvider<T>
        where T : class
    {
        private readonly List<Type> _providerTypes;
        
        public DIProvider()
        {
            _providerTypes = new List<Type>();
        }
        
        public IServiceProvider? Provider { get; set; }

        public void RegisterType(Type providerType)
        {
            if (!providerType.IsAssignableTo(typeof(T)))
            {
                throw new ArgumentException($@"Type {providerType.Name} cannot be added as it doesn't inherit from {typeof(T).FullName}");
            }
            
            _providerTypes.Add(providerType);
        }
        public IEnumerable<T> GetData()
        {
            if (Provider is null)
            {
                throw new InvalidOperationException("Register by type can only be done if service provider is used");
            }

            return _providerTypes
                .Select(x => Provider.GetRequiredService(x))
                .Cast<T>();
        }
    }
}