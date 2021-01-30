using System;

namespace XPInc.Hackathon.Core.Domain.Strategies
{
    public sealed class UnknowLevel : Level
    {
        public override string Code => "N/A";

        public override string Name => "Not Classified";

        public override bool ShouldFireNotification => false;

        public override TimeSpan AverageAllocationTime => TimeSpan.Zero;

        public override string Color => "#FFF";
    }
}