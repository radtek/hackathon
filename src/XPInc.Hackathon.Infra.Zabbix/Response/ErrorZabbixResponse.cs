using Newtonsoft.Json;

namespace XPInc.Hackathon.Infra.Zabbix.Response
{
    public class ErrorZabbixResponse : IZabbixHttpResponse
    {
        [JsonProperty("error")] public ErrorZabbix Error { get; set; }
        [JsonProperty("jsonrpc")] public string JsonRPC { get; set; }
        [JsonProperty("id")] public string Id { get; set; }


        public class ErrorZabbix
        {
            [JsonProperty("code")] public int Code { get; set; }
            [JsonProperty("message")] public string Message { get; set; }
            [JsonProperty("data")] public string Data { get; set; }
        }

    }
}
