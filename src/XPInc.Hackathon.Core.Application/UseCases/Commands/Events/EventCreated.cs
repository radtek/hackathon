using XPInc.Hackathon.Core.Domain;
using XPInc.Hackathon.Framework.CQRS.Commands;

namespace XPInc.Hackathon.Core.Application.UseCases.Commands.Events
{
    public sealed class EventCreated : ICommandEvent
    {
        public Event Event { get; private set; }

        public string Key => "zabbix-events-tape";

        public string Group => "NOC3X";

        public EventCreated(Event @event)
        {
            Event = @event;
        }
    }
}
