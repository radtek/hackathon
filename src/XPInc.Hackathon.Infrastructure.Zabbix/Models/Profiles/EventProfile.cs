using AutoMapper;
using XPInc.Hackathon.Core.Domain;
using XPInc.Hackathon.Core.Domain.Commands;
using static XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Response.EventResponse;

namespace XPInc.Hackathon.Infrastructure.Zabbix.Models.Profiles
{
    public sealed class IncidentProfile : Profile
    {
        public IncidentProfile()
        {
            CreateMap<EventZabbix, Event>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .ConstructUsing(_ =>
                    Event.Create(new CreateEventCommand(int.Parse(_.Severity)) // severity must be an integer
                    {
                        EventId = _.EventId,
                        Host = _.Source,
                        ProblemDescription = _.Name,
                        Username = _.UserId
                    }));
        }
    }
}
