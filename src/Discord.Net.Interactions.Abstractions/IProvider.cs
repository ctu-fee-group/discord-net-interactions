using System.Collections;
using System.Collections.Generic;

namespace Discord.Net.Interactions.Abstractions
{
    public interface IProvider<out T>
        where T : class
    {
        public IEnumerable<T> GetData();
    }
}