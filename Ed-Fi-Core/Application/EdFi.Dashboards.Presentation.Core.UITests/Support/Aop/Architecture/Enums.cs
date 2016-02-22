#if !POSTSHARP

using System;

namespace EdFi.Dashboards.Presentation.Core.UITests.Support.Aop.Architecture
{
    public enum FlowBehavior
    {
        Default,
        Continue,
        RethrowException,
        Return,
        ThrowException,
    }

    [Flags]
    public enum MulticastAttributes
    {
        Public = 1,
        Instance = 2,
        Virtual = 4,
    }
}

#endif