using System;

namespace Discord.Net.Interactions.Controllers.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumValueAttribute : Attribute
    {
        public string DisplayName { get; }
        
        public EnumValueAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}