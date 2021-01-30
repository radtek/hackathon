using System;

namespace XPInc.Hackathon.Core.Domain.Strategies
{
    public sealed class WarningLevel : Level
    {
        public override string Code => "P4";

        public override string Name => "Warning";

        public override bool ShouldFireNotification => false;

        public override TimeSpan AverageAllocationTime => TimeSpan.FromHours(1);

        public override string Color => "#FFF";
    }
}