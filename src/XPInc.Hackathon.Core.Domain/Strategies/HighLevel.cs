using System;

namespace XPInc.Hackathon.Core.Domain.Strategies
{
    public sealed class HighLevel : Level
    {
        public override string Code => "P2";

        public override string Name => "High";

        public override bool ShouldFireNotification => true;

        public override TimeSpan AverageAllocationTime => TimeSpan.FromMinutes(5);

        public override string Color => "#FFF";
    }
}