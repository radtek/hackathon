using System.Threading;
using System.Threading.Tasks;
using XPInc.Hackathon.Core.Application.UseCases.Commands.Events.Handlers.Abstractions;

namespace XPInc.Hackathon.Core.Application.UseCases.Commands.Events.Handlers
{
    public sealed class PersistEventCreatedHandler : IPersistEventCreatedHandler
    {
        public Task Handle(EventCreated notification, CancellationToken cancellationToken)
        {
            //TODO: persist at database (mongo)

            return Task.CompletedTask;
        }
    }
}
