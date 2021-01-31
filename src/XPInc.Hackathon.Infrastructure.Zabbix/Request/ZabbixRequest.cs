using RestSharp;

namespace XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Request
{
    public abstract class ZabbixRequest : RestRequest
    {
        private const string urlZabbix = "http://opti.xpinc.io/zabbix-hml/api_jsonrpc.php";

        protected ZabbixRequest(string action = urlZabbix, Method method = Method.GET) 
                         : base(action, method, DataFormat.Json)
        {
            AddHeader("Accept", "application/json");
        }
    }
}
