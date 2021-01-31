using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XPInc.Hackathon.Core.Domain;

namespace XPInc.Hackathon.Core.Application.Services
{
    public interface IIncidentService
    {
        Task<IEnumerable<Event>> GetIncidentsAsync()
            => this.GetIncidentsAsync(CancellationToken.None);

        Task<IEnumerable<Event>> GetIncidentsAsync(CancellationToken cancellationToken);

        Task AckAsync(IEnumerable<Event> events)
            => this.AckAsync(events, CancellationToken.None);

        Task<IEnumerable<string>> AckAsync(IEnumerable<Event> events, CancellationToken cancellationToken);
    }
}
