using System;

namespace Discord.Net.Interactions.Controllers.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SlashCommandAttribute : Attribute
    {
        public string Name { get; }
        
        public string? Description { get; init; }

        public SlashCommandAttribute(string name)
        {
            Name = name;
        }
    }
}