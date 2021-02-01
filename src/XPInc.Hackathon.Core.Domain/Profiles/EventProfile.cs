using AutoMapper;
using XPInc.Hackathon.Core.Domain.Commands;
using XPInc.Hackathon.Infrastructure.Zabbix.Response;

namespace XPInc.Hackathon.Core.Domain.Profiles
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
