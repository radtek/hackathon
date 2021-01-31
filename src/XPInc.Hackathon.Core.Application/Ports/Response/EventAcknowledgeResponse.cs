using Newtonsoft.Json;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Response;

namespace XPInc.Hackathon.Infrastructure.Zabbix.Response
{
    public sealed class EventAcknowledgeResponse : IZabbixHttpResponse
    {

        [JsonProperty("result")]
        public Result Result { get; set; }
    }

    public class Result
    {
        [JsonProperty("eventids")]
        public int[] EventIds { get; set; }
    }

}
