using AutoMapper;
using XPInc.Hackathon.Core.Domain;
using XPInc.Hackathon.Core.Domain.Commands;

namespace XPInc.Hackathon.Infrastructure.Zabbix.Models.Profiles
{
    public sealed class IncidentProfile : Profile
    {
        public IncidentProfile()
        {
            CreateMap<EventDto, Event>()
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
