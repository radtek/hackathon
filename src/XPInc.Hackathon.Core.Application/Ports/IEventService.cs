using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XPInc.Hackathon.Core.Domain;

namespace XPInc.Hackathon.Core.Application.Services
{
    public interface IEventService
    {
        Task<IEnumerable<Team>> GetTeamsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetEventsAsync(int externalId, CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> AckAsync(EventAckRequest request, CancellationToken cancellationToken = default);
    }
}
