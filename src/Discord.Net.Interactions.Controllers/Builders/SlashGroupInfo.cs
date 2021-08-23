using System;
using System.Collections.Generic;
using System.Reflection;
using Discord.Net.Interactions.Controllers.Attributes;

namespace Discord.Net.Interactions.Controllers.Builders
{
    public record SlashGroupInfo(
        SlashGroupInfo? Parent,
        SlashCommandInfo? Command,
        SlashGroupAttribute? GroupAttribute,
        IReadOnlyCollection<Attribute> Attributes,
        IReadOnlyCollection<SlashGroupInfo> Children);

    public record SlashCommandInfo(
        MethodInfo MethodInfo,
        SlashGroupInfo? Parent,
        SlashCommandAttribute Attribute);

    public record ControllerInfo(
        Type ControllerType,
        IReadOnlyCollection<SlashGroupInfo> TopLevelGroups
    );
}