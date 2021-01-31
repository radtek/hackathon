using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XPInc.Hackathon.Infra.Zabbix.Extensions;
using XPInc.Hackathon.Infra.Zabbix.Messages;
using XPInc.Hackathon.Infra.Zabbix.Request;
using XPInc.Hackathon.Infra.Zabbix.Response;

namespace XPInc.Hackathon.Infra.Zabbix.Services
{
    public class ZabbixService : IZabbixService
    {
        private readonly IRestClient _client;



        public ZabbixService(IRestClient client)
        {
            _client = client;
        }

        public Task<IEnumerable<string>> AckAsync(IEnumerable<EventModel> incidents, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task GetAlertsAsync(Guid guid)
        {
            throw new NotImplementedException();
        }

        public async Task<EventResponse> GetIncidentsAsync(int[] GroupIds, bool IsOpen = false, CancellationToken cancellationToken = default)
        {
            EventResponse response = null;
            await _client.ExecutePostAsync(new EventRequest(new EventMessage(GroupIds, IsOpen)), cancellationToken)
                              .ContinueWith(t =>
                                            {
                                                try
                                                {
                                                    if (t.Result != null && t.Result.StatusCode == HttpStatusCode.OK)
                                                        response = t.Result.Content.JsonAs<EventResponse>();
                                                }
                                                catch
                                                {
                                                    var error = t.Result.Content.JsonAs<ErrorZabbixResponse>();
                                                    //log error.Error
                                                }
                                            }, cancellationToken);
            return response;
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
