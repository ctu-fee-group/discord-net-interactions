using System;

namespace Discord.Net.Interactions.Controllers.Types
{
    public interface IControllerTypeResolver
    {
        public bool IsEnabledFor(Type type);

        public void AppendType(Type type, SlashCommandOptionBuilder optionBuilder);
    }

    public abstract class ControllerTypeResolver<T> : IControllerTypeResolver
    {
        public virtual bool IsEnabledFor(Type type)
        {
            return typeof(T) == type;
        }

        public virtual void AppendType(Type type, SlashCommandOptionBuilder optionBuilder)
        {
            AppendType(optionBuilder);
        }
        
        public abstract void AppendType(SlashCommandOptionBuilder optionBuilder);
    }
}