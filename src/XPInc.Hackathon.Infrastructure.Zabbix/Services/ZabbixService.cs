using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RestSharp;
using XPInc.Hackathon.Infrastructure.Zabbix.Enums;
using XPInc.Hackathon.Infrastructure.Zabbix.Filters;
using XPInc.Hackathon.Infrastructure.Zabbix.Response;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Extensions;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Messages;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Request;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Response;

namespace XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Services
{
    public class ZabbixService : IZabbixService
    {
        private readonly IRestClient _client;
        private readonly IMapper _mapper;


        public ZabbixService(IRestClient client, IMapper mapper)
        {
            _client = client ?? throw new ArgumentNullException(nameof(_client));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<string>> AckAsync(IEnumerable<EventZabbix> events, ActionAcknowledge? action = null, string message = "", SeverityAcknowledge? severity = null, CancellationToken cancellationToken = default)
        {
            var acked = new List<string>();
            message = "Problem resolved";
            action = action.HasValue ? action : (ActionAcknowledge)6;

            await _client.ExecutePostAsync(new EventRequest(new EventAcknowledgeMessage(
                                                                events.Select(_ => _.EventId).ToArray(),
                                                                (ActionAcknowledge)action,
                                                                message,
                                                                severity)
                                           ), cancellationToken)
                              .ContinueWith(t =>
                              {
                                  try
                                  {
                                      if (t.Result != null && t.Result.StatusCode == HttpStatusCode.OK)
                                      {
                                          var response = t.Result.Content.JsonAs<EventAcknowledgeResponse>();
                                          acked.AddRange(response.Result.EventIds.Select(x => x.ToString()));
                                      }
                                  }
                                  catch
                                  {
                                      var error = t.Result.Content.JsonAs<ErrorZabbixResponse>();
                                      //log error.Error
                                  }
                              }, cancellationToken);
            return acked;
        }

        public Task GetAlertsAsync(Guid guid)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<EventZabbix>> GetEventsAsync(EventZabbixFilter filter, CancellationToken cancellationToken = default)
        {
            var events = new List<EventZabbix>();

            Task task1 = _client.ExecutePostAsync(new EventRequest(new EventMessage(filter)), cancellationToken)
                              .ContinueWith(t =>
                                            {
                                                try
                                                {
                                                    if (t.Result != null && t.Result.StatusCode == HttpStatusCode.OK)
                                                    {
                                                        var result = t.Result.Content.JsonAs<EventResponse>();
                                                        events.AddRange(result.Events);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    var error = t.Result.Content.JsonAs<ErrorZabbixResponse>();
                                                    //log error.Error
                                                }
                                            }, cancellationToken);
            await Task.WhenAll(task1);
            return events;
        }

        public async Task<LoginResponse> LoginAsync(string user, string pass, CancellationToken cancellationToken = default)
        {
            LoginResponse response = null;
            user = string.IsNullOrEmpty(user) ? "Admin" : user;
            pass = string.IsNullOrEmpty(pass) ? "zabbix" : pass;

            await _client.ExecutePostAsync(new LoginZabbixRequest(new LoginMessage(user, pass)))
                   .ContinueWith(t =>
                   {
                       if (t.Result != null && t.Result.StatusCode == HttpStatusCode.OK)
                       {
                           response = t.Result.Content.JsonAs<LoginResponse>();
                           //Log.Information($"Result of {nameof(LoginAsync)}: {response.AsJson()}")
                       }
                       else
                       {
                           //Log.Error($"Error to call {nameof(LoginAsync)}")
                       }
                   });
            return response;
        }
    }
}
