using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XPInc.Hackathon.Core.Domain;

namespace XPInc.Hackathon.Core.Application
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetEventsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> AckAsync(IEnumerable<Event> incidents, CancellationToken cancellationToken = default);
    }
}