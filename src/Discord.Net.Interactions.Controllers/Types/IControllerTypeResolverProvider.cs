using System.Collections;
using System.Collections.Generic;

namespace Discord.Net.Interactions.Controllers.Types
{
    public interface IControllerTypeResolverProvider
    {
        public IEnumerable<IControllerTypeResolver> GetResolvers();
    }
}