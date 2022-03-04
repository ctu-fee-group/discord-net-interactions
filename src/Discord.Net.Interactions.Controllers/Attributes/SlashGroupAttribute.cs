using System;

namespace Discord.Net.Interactions.Controllers.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SlashGroupAttribute : Attribute
    {
        public string Name { get; }
        
        public string? Description { get; init; }

        public SlashGroupAttribute(string name)
        {
            Name = name;
        }
    }
}