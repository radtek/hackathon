using Newtonsoft.Json;
using XPInc.Hackathon.Infra.Zabbix.Messages.Base;
using XPInc.Hackathon.Infra.Zabbix.Messages.Request;

namespace XPInc.Hackathon.Infra.Zabbix.Messages
{
    public class HttpSendMessageBase<T> : HttpMessageBase<RequestBody<T>> where T : class
    {
        [JsonProperty("params", Order = 3)]
        public T Message { get; set; }
    }
}
