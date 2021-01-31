using Newtonsoft.Json;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Messages.Base;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Messages.Request;

namespace XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Messages
{
    public class HttpSendMessageBase<T> : HttpMessageBase<RequestBody<T>> where T : class
    {
        [JsonProperty("params", Order = 3)]
        public T Message { get; set; }
    }
}
