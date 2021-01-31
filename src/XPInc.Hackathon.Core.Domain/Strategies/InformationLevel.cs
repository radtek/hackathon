using System;

namespace XPInc.Hackathon.Core.Domain.Strategies
{
    public sealed class InformationLevel : EventLevel
    {
        public override string Code => "PI";

        public override string Name => "Information";

        public override bool ShouldFireNotification => false;

        public override TimeSpan AverageAllocationTime => TimeSpan.Zero;

        public override string Color => "#FFF";
    }
}