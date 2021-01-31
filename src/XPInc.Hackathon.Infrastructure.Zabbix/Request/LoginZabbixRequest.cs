using RestSharp;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Messages;

namespace XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Request
{
    public class LoginZabbixRequest : ZabbixRequest
    {
        public LoginZabbixRequest(LoginMessage message) : base()
        {
            AddParameter("application/json", message.ToString(), ParameterType.RequestBody);
        }
    }
}
