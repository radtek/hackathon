using Newtonsoft.Json;

namespace XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Response
{
    public class ZabbixHttpResponse<T> : IZabbixHttpResponse
    {
        [JsonProperty("result")] public T Result { get; set; }
        [JsonProperty("jsonrpc")] public string JsonRPC { get; set; }
        [JsonProperty("id")] public string Id { get; set; }
    }
}
