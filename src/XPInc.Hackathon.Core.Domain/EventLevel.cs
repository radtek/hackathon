using System;

namespace XPInc.Hackathon.Core.Domain
{
    public abstract class EventLevel
    {
        public virtual string Code { get; set; }

        public virtual string Name { get; set; }

        public virtual bool ShouldFireNotification { get; set; }

        public virtual TimeSpan AverageAllocationTime { get; set; }

        public virtual string Color { get; set; }
    }
}
