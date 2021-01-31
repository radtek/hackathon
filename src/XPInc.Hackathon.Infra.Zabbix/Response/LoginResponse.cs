using Newtonsoft.Json;
using System;

namespace XPInc.Hackathon.Infra.Zabbix.Response
{
    public class LoginResponse: IZabbixHttpResponse
    {
        [JsonProperty("result")] public Guid? Auth { get; set; }
    }
}
