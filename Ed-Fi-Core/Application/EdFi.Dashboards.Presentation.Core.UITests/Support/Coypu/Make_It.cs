using System;
using Coypu;

namespace EdFi.Dashboards.Presentation.Core.UITests.Support.Coypu
{
    public static class Make_It
    {
        public static readonly Options Wait_1_Second = new Options {Timeout = TimeSpan.FromSeconds(1)};
        public static readonly Options Wait_5_Seconds = new Options { Timeout = TimeSpan.FromSeconds(5) };
        public static readonly Options Wait_10_Seconds = new Options { Timeout = TimeSpan.FromSeconds(10) };
        public static readonly Options Consider_Invisible_Elements = new Options { ConsiderInvisibleElements = true };
        public static readonly Options Do_It_Now = new Options {Timeout = TimeSpan.Zero, RetryInterval = TimeSpan.Zero};
    }
}
