using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XPInc.Hackathon.Infrastructure.Zabbix.Enums;
using XPInc.Hackathon.Infrastructure.Zabbix.Filters;
using XPInc.Hackathon.Infrastructure.Zabbix.Response;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Response;


namespace XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Services
{
    public interface IZabbixService
    {
        Task<LoginResponse> LoginAsync(string user, string pass, CancellationToken cancellationToken = default);

        Task<IEnumerable<EventZabbix>> GetEventsAsync(EventZabbixFilter request, CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> AckAsync(IEnumerable<EventZabbix> events,
                                                        ActionAcknowledge? action = null,
                                                        string message = "",
                                                        SeverityAcknowledge? severity = null,
                                                        CancellationToken cancellationToken = default);

    }
}
