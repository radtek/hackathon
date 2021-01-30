using System;

namespace XPInc.Hackathon.Core.Domain.Strategies
{
    public sealed class AverageLevel : Level
    {
        public override string Code => "P3";

        public override string Name => "Average";

        public override bool ShouldFireNotification => false;

        public override TimeSpan AverageAllocationTime => TimeSpan.FromHours(1);

        public override string Color => "#FFF";
    }
}