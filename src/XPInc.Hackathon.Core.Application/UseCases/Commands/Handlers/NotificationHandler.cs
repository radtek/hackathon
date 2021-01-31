using System.Threading;
using System.Threading.Tasks;
using MediatR;
using XPInc.Hackathon.Core.Application.UseCases.Commands.Handlers.Abstractions;
using XPInc.Hackathon.Core.Domain.Repositories;

namespace XPInc.Hackathon.Core.Application.UseCases.Commands.Handlers
{
    public class NotificationHandler : INotificationHandler
    {
        private readonly IEventRepository _eventRepository;

        public NotificationHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public Task<Unit> Handle(NotificationCommand request, CancellationToken cancellationToken)
        {

        }
    }
}
