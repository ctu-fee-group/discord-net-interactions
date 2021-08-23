using System;
using System.Collections.Generic;
using System.Reflection;

namespace Discord.Net.Interactions.Controllers.Builders
{
    public record ControllerInteractionInfo
    (
        IReadOnlyCollection<Attribute> Attributes,
        ControllerInfo Controller
    );
}