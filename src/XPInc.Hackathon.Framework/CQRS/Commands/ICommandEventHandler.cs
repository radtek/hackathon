using MediatR;

namespace XPInc.Hackathon.Framework.CQRS.Commands
{
    public interface ICommandEventHandler<in TCommandEvent> : INotificationHandler<TCommandEvent>
         where TCommandEvent : ICommandEvent
    { }
}
