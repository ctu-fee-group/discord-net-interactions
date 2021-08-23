namespace Discord.Net.Interactions.Controllers.Types
{
    public class LongControllerResolver : ControllerTypeResolver<long>
    {
        public override void AppendType(SlashCommandOptionBuilder optionBuilder)
        {
            optionBuilder.WithType(ApplicationCommandOptionType.Integer);
        }
    }
    
    public class StringControllerResolver : ControllerTypeResolver<string>
    {
        public override void AppendType(SlashCommandOptionBuilder optionBuilder)
        {
            optionBuilder.WithType(ApplicationCommandOptionType.String);
        }
    }
    
    public class BooleanControllerResolver : ControllerTypeResolver<bool>
    {
        public override void AppendType(SlashCommandOptionBuilder optionBuilder)
        {
            optionBuilder.WithType(ApplicationCommandOptionType.Boolean);
        }
    }
    
    public class ChannelControllerResolver : ControllerTypeResolver<IChannel>
    {
        public override void AppendType(SlashCommandOptionBuilder optionBuilder)
        {
            optionBuilder.WithType(ApplicationCommandOptionType.Channel);
        }
    }
    
    public class MentionableControllerResolver : ControllerTypeResolver<IMentionable>
    {
        public override void AppendType(SlashCommandOptionBuilder optionBuilder)
        {
            optionBuilder.WithType(ApplicationCommandOptionType.Mentionable);
        }
    }
    
    public class UserControllerResolver : ControllerTypeResolver<IUser>
    {
        public override void AppendType(SlashCommandOptionBuilder optionBuilder)
        {
            optionBuilder.WithType(ApplicationCommandOptionType.User);
        }
    }
    
    public class RoleControllerResolver : ControllerTypeResolver<IRole>
    {
        public override void AppendType(SlashCommandOptionBuilder optionBuilder)
        {
            optionBuilder.WithType(ApplicationCommandOptionType.Role);
        }
    }
    
    public class DoubleControllerResolver : ControllerTypeResolver<double>
    {
        public override void AppendType(SlashCommandOptionBuilder optionBuilder)
        {
            optionBuilder.WithType(ApplicationCommandOptionType.Number);
        }
    }
}