using System.Collections.Generic;
using Newtonsoft.Json;

namespace XPInc.Hackathon.Infrastructure.Zabbix.Models
{
    public sealed class EventResult
    {
        [JsonProperty("result")]
        public IEnumerable<EventDto> Content { get; set; }
    }

    public sealed class EventDto
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
    }
}
