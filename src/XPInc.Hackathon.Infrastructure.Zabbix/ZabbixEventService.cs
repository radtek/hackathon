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
    public sealed class ZabbixEventService : IEventService
    {
        private static readonly object s_lock = new object();
        private static string s_token;

        private const string Route = "api_jsonrpc.php";

        public static string Token
        {
            get { return s_token; }
            set
            {
                lock (s_lock)
                {
                    s_token = value;
                }
            }
        }

        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public ZabbixEventService(HttpClient httpClient, IMapper mapper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<IEnumerable<Team>> GetTeamsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Event>> GetEventsAsync(CancellationToken cancellationToken = default)
        {
            Token = await AuthenticateAsync(cancellationToken).ConfigureAwait(false); // get a fresh new token

            var body = new
            {
                jsonrpc = "2.0",
                method = "event.get",
                @params = new
                {
                    groupids = new[] { "65" }, //TODO: get all groups
                    acknowledged = false,
                    sortfield = new[] { "clock" }, // filter by time of creation
                    sortorder = "asc" // filter oldest first
                },
                auth = Token,
                id = 1
            };
            var content = new StringContent(body.ToJson());
            var message = new HttpRequestMessage(HttpMethod.Get, Route) { Content = content };
            var response = await _httpClient.SendAsync(message, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            { return default; }

            var responseAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = responseAsString.FromJson<EventResult>();
            var events = new List<Event>();

            result.Content
                .ToList().ForEach(item => events.Add(_mapper.Map<Event>(item))); // map result to domain type

            return events;
        }

        public async Task<IEnumerable<string>> AckAsync(IEnumerable<Event> incidents, CancellationToken cancellationToken = default)
        {
            Token = await AuthenticateAsync(cancellationToken).ConfigureAwait(false); // get a fresh new token

            var body = new
            {
                jsonrpc = "2.0",
                method = "event.acknowledge",
                @params = new
                {
                    eventids = incidents.Select(_ => _.EventId),
                    message = "Problem resolved"
                },
                auth = Token,
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
