using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XPInc.Hackathon.Core.Domain;

namespace XPInc.Hackathon.Core.Application
{
    public interface IIncidentService
    {
        Task<IEnumerable<Incident>> GetIncidentsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> AckAsync(IEnumerable<Incident> incidents, CancellationToken cancellationToken = default);
    }
}