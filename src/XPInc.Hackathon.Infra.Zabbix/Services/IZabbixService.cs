using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XPInc.Hackathon.Infra.Zabbix.Messages;
using XPInc.Hackathon.Infra.Zabbix.Response;

namespace XPInc.Hackathon.Infra.Zabbix.Services
{
    public interface IZabbixService
    {
        Task<LoginResponse> LoginAsync(string user, string pass, CancellationToken cancellationToken = default);

        Task<EventResponse> GetIncidentsAsync(int[] groupids, bool isopen = true, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> AckAsync(IEnumerable<EventModel> incidents, CancellationToken cancellationToken = default);





    }
}
