using System;

namespace XPInc.Hackathon.Core.Domain.Strategies
{
    public sealed class DisasterLevel : Level
    {
        public override string Code => "P1";

        public override string Name => "Disaster";

        public override bool ShouldFireNotification => true;

        public override TimeSpan AverageAllocationTime => TimeSpan.FromMinutes(3);

        public override string Color => "#FFF";
    }
}