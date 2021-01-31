using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XPInc.Hackathon.Core.Domain;

namespace XPInc.Hackathon.Core.Application.Services
{
    public interface IEventService
    {
        Task<LoginResponse> LoginAsync(string user, string pass, CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetEventsAsync(EventFilterRequest request, CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> AckAsync(EventAckRequest request, CancellationToken cancellationToken = default);
    }
}
