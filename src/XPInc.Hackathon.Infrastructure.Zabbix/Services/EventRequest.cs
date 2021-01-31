using RestSharp;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Messages;

namespace XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Services
{
    internal class EventRequest : IRestRequest
    {
        private EventMessage eventMessage;

        public EventRequest(EventMessage eventMessage)
        {
            this.eventMessage = eventMessage;
        }
    }
}