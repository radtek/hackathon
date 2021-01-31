using XPInc.Hackathon.Framework.CQRS.Commands;

namespace XPInc.Hackathon.Core.Application.UseCases.Commands.Events.Handlers.Abstractions
{
    public interface IPersistEventCreatedHandler : ICommandEventHandler<EventCreated>
    { }
}
