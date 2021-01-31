using RestSharp;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Messages;

namespace XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Request
{
    public sealed class EventRequest : ZabbixRequest
    {
        public EventRequest(IEventMessage message) : base()
        {
            AddParameter("application/json", message.ToString(), ParameterType.RequestBody);
        }
    }
}
