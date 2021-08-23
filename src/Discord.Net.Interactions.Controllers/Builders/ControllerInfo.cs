using System;
using System.Collections.Generic;

namespace Discord.Net.Interactions.Controllers.Builders
{
    public record ControllerInfo
    (
        IReadOnlyCollection<Attribute> Attributes,
        IReadOnlyCollection<ControllerInteractionInfo> InteractionInfos
    );
}