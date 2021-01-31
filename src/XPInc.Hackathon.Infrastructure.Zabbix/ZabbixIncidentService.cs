using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using XPInc.Hackathon.Core.Application;
using XPInc.Hackathon.Core.Domain;
using XPInc.Hackathon.Framework.Extensions;
using XPInc.Hackathon.Infrastructure.Zabbix.Models;

namespace XPInc.Hackathon.Infrastructure.Zabbix
{
    public sealed class ZabbixIncidentService : IIncidentService
    {
        private static readonly object _lock = new object();
        private static string s_token;

        private const string Route = "zabbix-hml/api_jsonrpc.php";

        public static string Token
        {
            get { return s_token; }
            set
            {
                lock (_lock)
                {
                    s_token = value;
                }
            }
        }

        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public ZabbixIncidentService(HttpClient httpClient, IMapper mapper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<Incident>> GetIncidentsAsync(CancellationToken cancellationToken = default)
        {
            if (Token == default)
            {
                Token = await AuthenticateAsync(cancellationToken).ConfigureAwait(false); // get a fresh new token
            }

            var body = new
            {
                jsonrpc = "2.0",
                method = "event.get",
                @params = new
                {
                    limit = 50
                },
                id = 1
            };
            var content = new StringContent(body.ToJson());
            var message = new HttpRequestMessage(HttpMethod.Get, Route) { Content = content };
            var response = await _httpClient.SendAsync(message, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            { return default; }

            var responseAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = responseAsString.FromJson<EventResult>();
            var incidents = new List<Incident>();

            result.Content
                .ToList().ForEach(item => incidents.Add(_mapper.Map<Incident>(item))); // map result to domain type

            return incidents;
        }

        public async Task<IEnumerable<string>> AckAsync(IEnumerable<Incident> incidents, CancellationToken cancellationToken = default)
        {
            if (Token == default)
            {
                Token = await AuthenticateAsync(cancellationToken).ConfigureAwait(false); // get a fresh new token
            }

            var body = new
            {
                jsonrpc = "2.0",
                method = "event.acknowledge",
                @params = new
                {
                    eventids = incidents.Select(_ => _.EventId),
                    message = "Problem resolved"
                },
                id = 1
            };
            var content = new StringContent(body.ToJson());
            var message = new HttpRequestMessage(HttpMethod.Get, Route) { Content = content };
            var response = await _httpClient.SendAsync(message, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            { return default; }

            var responseAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = responseAsString.FromJson<dynamic>();
            var acked = new List<string>();

            foreach (var id in result.result.eventIds)
            {
                acked.Add(id);
            }

            return acked;
        }

        private async Task<string> AuthenticateAsync(CancellationToken cancellationToken = default)
        {
            var body = new
            {
                jsonrpc = "2.0",
                method = "user.login",
                @params = new
                {
                    user = "Admin",
                    password = "zabbix"
                },
                id = 1
            };
            var content = new StringContent(body.ToJson()); // body content
            var message = new HttpRequestMessage(HttpMethod.Get, Route) { Content = content }; // http request
            var response = await _httpClient.SendAsync(message, cancellationToken).ConfigureAwait(false); // make request

            response.EnsureSuccessStatusCode(); // throws if response is all but 2xx

            var responseAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = responseAsString.FromJson<dynamic>();

            return result.result;
        }
    }
}
