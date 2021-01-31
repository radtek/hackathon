using RestSharp;
using XPInc.Hackathon.Infra.Zabbix.Messages;

namespace XPInc.Hackathon.Infra.Zabbix.Request
{
    public class LoginZabbixRequest : ZabbixRequest
    {
        public LoginZabbixRequest(LoginMessage message) : base()
        {
            AddParameter("application/json", message.ToString(), ParameterType.RequestBody);
        }
    }
}
