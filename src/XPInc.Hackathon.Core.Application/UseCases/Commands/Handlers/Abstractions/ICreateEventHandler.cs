using XPInc.Hackathon.Framework.CQRS.Commands;

namespace XPInc.Hackathon.Core.Application.UseCases.Commands.Handlers.Abstractions
{
    public interface ICreateEventHandler : ICommandHandler<CreateEvent>
    { }
}
