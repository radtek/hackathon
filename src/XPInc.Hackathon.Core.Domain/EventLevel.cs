using System;

namespace XPInc.Hackathon.Core.Domain
{
    public abstract class EventLevel
    {
        public abstract string Code { get; }

        public abstract string Name { get; }

        public abstract bool ShouldFireNotification { get; }

        public abstract TimeSpan AverageAllocationTime { get; }

        public abstract string Color { get; }
    }
}