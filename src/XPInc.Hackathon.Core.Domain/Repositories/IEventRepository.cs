using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace XPInc.Hackathon.Core.Domain.Repositories
{
    public interface IEventRepository
    {
        Task SaveEventsAsync(IEnumerable<Event> events, CancellationToken cancellationToken = default);
        Task<IEnumerable<AlertDefinition>> GetAlertDefinitionsAsync(CancellationToken cancellationToken);
    }
}
