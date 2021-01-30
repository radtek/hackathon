using MediatR;

namespace XPInc.Hackathon.Framework.CQRS.Commands
{
    public interface ICommandEvent : INotification
    { }
}
