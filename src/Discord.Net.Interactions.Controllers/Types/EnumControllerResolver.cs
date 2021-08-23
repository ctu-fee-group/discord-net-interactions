using System;
using System.Reflection;
using Discord.Net.Interactions.Controllers.Attributes;
using Discord.Net.Interactions.Controllers.Extensions;

namespace Discord.Net.Interactions.Controllers.Types
{
    public class EnumControllerResolver : IControllerTypeResolver
    {
        public bool IsEnabledFor(Type type)
        {
            return type.IsEnum;
        }

        public void AppendType(Type type, SlashCommandOptionBuilder optionBuilder)
        {
            if (type.IsAssignableTo(typeof(long)))
            {
                throw new InvalidOperationException("Only enums that derive from long can be used for now");
            }

            optionBuilder.WithType(ApplicationCommandOptionType.Integer);
            
            foreach (var item in type.GetEnumValues())
            {
                if (item is null)
                {
                    continue;
                }

                MemberInfo[] memberInfo = type.GetMember(type.GetEnumName(item) ??
                                                         throw new InvalidOperationException("Could not obtain enum item name"));
                EnumValueAttribute? attribute = memberInfo[0].GetCustomAttribute<EnumValueAttribute>();
                
                optionBuilder.AddChoice(attribute?.DisplayName ?? type.GetEnumName(item), (int)item);
            }
        }
    }
}