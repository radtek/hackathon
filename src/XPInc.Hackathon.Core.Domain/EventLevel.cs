using System;

namespace XPInc.Hackathon.Core.Domain
{
    public class EventLevel
    {
        public string Code { get; }

        public string Name { get; }

        public bool ShouldFireNotification { get; }

        public TimeSpan AverageAllocationTime { get; }

        public string Color { get; }

        public EventLevel(string code, string name)
        {
            Code = code;
            Name = name;
        }
    }
}
