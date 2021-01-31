using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XPInc.Hackathon.Core.Domain;
using XPInc.Hackathon.Infrastructure.Zabbix.Enums;
using XPInc.Hackathon.Infrastructure.Zabbix.Models.Request;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Response;

namespace XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Services
{
    public interface IZabbixService
    {
        Task<LoginResponse> LoginAsync(string user, string pass, CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetEventsAsync(EventFilterRequest request, CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> AckAsync(IEnumerable<Event> events,
                                                        ActionAcknowledge? action = null,
                                                        string message = "",
                                                        SeverityAcknowledge? severity = null,
                                                        CancellationToken cancellationToken = default);

    }
}
