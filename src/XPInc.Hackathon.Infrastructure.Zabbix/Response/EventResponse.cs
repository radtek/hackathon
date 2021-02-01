using System.Collections.Generic;
using Newtonsoft.Json;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Response;

namespace XPInc.Hackathon.Infrastructure.Zabbix.Response
{
    public class EventResponse : IZabbixHttpResponse
    {
        [JsonProperty("result")]
        public List<EventZabbix> Events { get; set; }
    }
    public class EventZabbix
    {
        [JsonProperty("eventid")]
        public string EventId { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("objectid")]
        public string ObjectId { get; set; }

        [JsonProperty("clock")]
        public string Clock { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("acknowledge")]
        public string Acknowledge { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("severity")]
        public string Severity { get; set; }

        [JsonProperty("userid")]
        public string UserId { get; set; }

        public UrlModel[] Urls { get; set; }
    }

    public class UrlModel
    {
        [JsonProperty("url")] public string Url { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
    }
}
