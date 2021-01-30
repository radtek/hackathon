using MediatR;

namespace XPInc.Hackathon.Framework.CQRS.Commands
{
    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand>
        where TCommand : ICommand
    { }
}
