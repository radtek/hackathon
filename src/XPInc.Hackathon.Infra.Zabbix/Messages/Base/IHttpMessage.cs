using Newtonsoft.Json;

namespace XPInc.Hackathon.Infra.Zabbix.Messages.Base
{
    public interface IHttpMessage<T>
    {
        [JsonProperty("id")] int Id { get; set; }

        [JsonProperty("method")] public string Method { get; set; }

        [JsonProperty("jsonrpc")] string JsonRPC { get; set; }

        [JsonProperty("auth")] string Auth { get; set; }

    }
}
