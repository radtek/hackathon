using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using XPInc.Hackathon.Core.Application;
using XPInc.Hackathon.Core.Domain;

namespace XPInc.Hackathon.Infrastructure.Zabbix
{
    public sealed class ZabbixIncidentService : IIncidentService
    {
        private readonly HttpClient _httpClient;

        public ZabbixIncidentService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public Task<IEnumerable<Incident>> GetIncidentsAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<string>> AckAsync(IEnumerable<Incident> incidents, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
