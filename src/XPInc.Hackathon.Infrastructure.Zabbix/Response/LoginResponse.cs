using Newtonsoft.Json;
using System;

namespace XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Response
{
    public class LoginResponse : IZabbixHttpResponse
    {
        [JsonProperty("result")] public Guid? Auth { get; set; }
    }
}
