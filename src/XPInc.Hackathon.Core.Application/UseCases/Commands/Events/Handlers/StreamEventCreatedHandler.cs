using System;
using System.Threading;
using System.Threading.Tasks;
using XPInc.Hackathon.Core.Application.UseCases.Commands.Events.Handlers.Abstractions;
using XPInc.Hackathon.Framework.Streaming;

namespace XPInc.Hackathon.Core.Application.UseCases.Commands.Events.Handlers
{
    public sealed class StreamEventCreatedHandler : IStreamEventCreatedHandler
    {
        private readonly IStreamingBroker _streamingBroker;

        public StreamEventCreatedHandler(IStreamingBroker streamingBroker)
        {
            _streamingBroker = streamingBroker ?? throw new ArgumentNullException(nameof(streamingBroker));
        }

        public Task Handle(EventCreated notification, CancellationToken cancellationToken)
        {
            // try to create a new stream tape
            _streamingBroker.CreateGroup(notification.Key, notification.Group);

            // add the new event to the stream
            _streamingBroker.Add(notification.Key, notification.Event);

            return Task.CompletedTask;
        }
    }
}
