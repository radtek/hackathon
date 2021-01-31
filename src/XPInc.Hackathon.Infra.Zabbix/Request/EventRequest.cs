using RestSharp;
using XPInc.Hackathon.Infra.Zabbix.Messages;

namespace XPInc.Hackathon.Infra.Zabbix.Request
{
    public sealed class EventRequest : ZabbixRequest
    {
        public EventRequest(EventMessage message) : base()
        {
            AddParameter("application/json", message.ToString(), ParameterType.RequestBody);
        }
    }
}
