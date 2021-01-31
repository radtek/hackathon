using Newtonsoft.Json;
using System;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Attributes;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Extensions;

namespace XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Messages.Base
{
    public class HttpMessageBase<T> : IHttpMessage<T>
    {
        [JsonProperty("jsonrpc", Order = 1)]
        public virtual string JsonRPC { get; set; } = "2.0";

        [JsonProperty("method", Order = 2)]
        public virtual string Method { get; set; } = GetMethod();

        [JsonProperty("auth", Order = 4)]
        public string Auth { get; set; }

        [JsonProperty("id", Order = 5)]
        public virtual int Id { get; set; } = 1;

        private static string GetMethod()
        {
            var type = typeof(T).GetGenericArguments()[0];
            var r = type.GetAttributeValue((MethodAttribute attr) => attr) as MethodAttribute;
            return r.MethodType.GetDisplayName();
        }

        public override string ToString()
        {
            return this.AsJson();
        }

    }
}
