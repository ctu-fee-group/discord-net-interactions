using System;
using System.Collections.Generic;
using System.Reflection;
using Discord.Net.Interactions.Controllers.Attributes;

namespace Discord.Net.Interactions.Controllers.Builders
{
    public record CommandGroupInfo(
        CommandGroupInfo? Parent,
        SlashCommandInfo? Command,
        SlashGroupAttribute? Group,
        IReadOnlyCollection<Attribute> Attributes,
        IReadOnlyCollection<CommandGroupInfo> Children);

    public record SlashCommandInfo(
        MethodInfo MethodInfo,
        CommandGroupInfo? Parent,
        SlashCommandAttribute Attribute);
}