using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using XPInc.Hackathon.Core.Application.Services;
using XPInc.Hackathon.Core.Application.UseCases.Commands.Events;
using XPInc.Hackathon.Core.Application.UseCases.Commands.Handlers.Abstractions;

namespace XPInc.Hackathon.Core.Application.UseCases.Commands.Handlers
{
    public sealed class CreateEventHandler : ICreateEventHandler
    {
        private readonly IEventService _eventService;
        private readonly IMediator _mediatr;

        public CreateEventHandler(IEventService eventService, IMediator mediatr)
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _mediatr = mediatr ?? throw new ArgumentNullException(nameof(mediatr));
        }

        public async Task<Unit> Handle(CreateEvent request, CancellationToken cancellationToken)
        {
            var result = await _eventService.GetEventsAsync(cancellationToken).ConfigureAwait(false);
            var @event = new EventCreated(result);

            _ = _mediatr.Publish(@event, cancellationToken); // fire event created (and forget)

            return Unit.Value;
        }
    }
}
